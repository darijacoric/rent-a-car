using RentACar.DAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RentACar.Models;
using RentACar.ViewModels;
using System.Net;
using RentACar.App_Helpers;
using Microsoft.AspNet.Identity;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Pdf.IO;
using System.Data.Entity;

namespace RentACar.Controllers
{
    // for TempData look: https://www.codeproject.com/articles/818493/mvc-tempdata-peek-and-keep-confusion

    [RequireHttps]
    public class HomeController : Controller
    {
        private string OrderSession = "OrderSessionKey";

        private RcDbContext _db = new RcDbContext();
        private UserDbContext _dbUser = new UserDbContext();

        // todo: Add when user selects Home breadcrumb, selected data is caried to Action 
        [AllowAnonymous]
        public ActionResult Index()
        {
            DateAndLocationViewModel model = new DateAndLocationViewModel();

            try
            {
                var locations = _db.VehicleLocation
                .Where(item => item.Status != Models.EntityStatus.Archived)
                .ToList();

                if (locations.Count() > 0)
                {
                    var cities = SetLocationCitiesDropDown(locations);
                    string selectedCity = cities.First().Text;
                    ViewBag.ReceiveCitiesDDW = cities;
                    ViewBag.ReceivePlaceDDW = SetLocationPlaceDropDown(locations, selectedCity);

                    cities = SetLocationCitiesDropDown(locations);
                    selectedCity = cities.First().Text;
                    ViewBag.DestinationCitiesDDW = cities;
                    ViewBag.DestinationPlaceDDW = SetLocationPlaceDropDown(locations, selectedCity);
                }
                else
                {
                    return View("~/Views/Shared/Maintenance.cshtml");
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DateAndLocation(DateAndLocationViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model.ReceiveTime < model.DestinationTime)
            {
                Order order = new Order();

                // Get location ids from DB from selected model data.
                var locations = _db.VehicleLocation
                    .Where(loc => loc.City.ToLower() == model.ReceiveCity.ToLower() && loc.Place.ToLower() == model.ReceivePlace.ToLower() ||
                        loc.City.ToLower() == model.DestinationCity.ToLower() && loc.Place.ToLower() == model.DestinationPlace.ToLower())
                    .ToList();

                order.ReceiveLocation = locations.Single(loc => loc.City.ToLower() == model.ReceiveCity.ToLower() && loc.Place.ToLower() == model.ReceivePlace.ToLower());
                order.RecvLocationId = order.ReceiveLocation.ID;
                order.RecvTime = model.ReceiveTime;

                order.DestinationLocation = locations.Single(loc => loc.City.ToLower() == model.DestinationCity.ToLower() && loc.Place.ToLower() == model.DestinationPlace.ToLower());
                order.DestLocationId = order.DestinationLocation.ID;
                order.DestTime = model.DestinationTime;

                int? orderNumber = _db.Orders
                .Select(o => o.OrderNumber).DefaultIfEmpty(0).Max() + 1;

                order.OrderNumber = orderNumber.HasValue ? orderNumber.Value : 1;

                Session[OrderSession] = order;

                return RedirectToAction("VehicleSelection");
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Pickup time cannot be higher or equal to Drop off time!");
            }

            var locationList = _db.VehicleLocation
               .Where(item => item.Status != Models.EntityStatus.Archived)
               .ToList();

            if (locationList.Count() > 0)
            {
                var cities = SetLocationCitiesDropDown(locationList);
                string selectedCity = cities.First().Text;
                ViewBag.ReceiveCitiesDDW = cities;
                ViewBag.ReceivePlaceDDW = SetLocationPlaceDropDown(locationList, selectedCity);

                cities = SetLocationCitiesDropDown(locationList);
                selectedCity = cities.First().Text;
                ViewBag.DestinationCitiesDDW = cities;
                ViewBag.DestinationPlaceDDW = SetLocationPlaceDropDown(locationList, selectedCity);
            }

            return View("~/Views/Home/Index.cshtml", model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult VehicleSelection(string vehicleType = null, string brand = null, int? consumptionFrom = null, int? consumptionTo = null, int? priceFrom = null, int? priceTo = null)
        {
            if (vehicleType == null || vehicleType == AppValues.ALL) { vehicleType = ""; }

            if (brand == null || brand == AppValues.ALL) { brand = ""; }

            if (consumptionFrom == null) { consumptionFrom = 0; }

            if (consumptionTo == null) { consumptionTo = int.MaxValue; }

            if (priceFrom == null) { priceFrom = 0; }

            if (priceTo == null) { priceTo = int.MaxValue; }

            Order order = (Order)Session[OrderSession];

            var vehicles = _db.Vehicles
                .Where(v => v.Status != EntityStatus.Archived && v.Status != EntityStatus.Active &&
                    v.RegistrationExpire > order.DestTime.Date &&
                    v.VehicleType.Contains(vehicleType) &&
                    v.Brand.Contains(brand) &&
                    (int)v.GasPerKm >= consumptionFrom && (int)v.GasPerKm <= consumptionTo &&
                    (int)v.DayPrice >= priceFrom && (int)v.DayPrice <= priceTo)
                    .OrderBy(v => v.DayPrice)
                .Include(v => v.Fuel)
                .ToList();


            HashSet<int> vehiclesHS = new HashSet<int>(vehicles.Select(item => item.ID));

            var carsEquipment = _db.CarEquipment
                .Where(item => vehiclesHS.Contains(item.VehicleID) && item.Status != EntityStatus.Archived)
                .ToList();

            List<VehicleSelectionViewModel> modelList = new List<VehicleSelectionViewModel>();
            VehicleSelectionViewModel model;

            foreach (var item in vehicles)
            {
                model = SetVehicleSelectionViewModel(item, order, vehiclesHS, carsEquipment);

                model.Fuel = item.Fuel.FuelName;

                modelList.Add(model);
            }


            ViewBag.ReceiveDate = order.RecvTime;
            ViewBag.DestinationDate = order.DestTime;
            // Select option all by default
            ViewBag.VehicleTypesDropDown = SetVehicleTypeDropDown(String.IsNullOrEmpty(vehicleType) ? vehicleType : AppValues.ALL);
            // Select option all by default
            ViewBag.BrandsDropDown = SetBrandDropDown(String.IsNullOrEmpty(brand) ? brand : AppValues.ALL);

            ViewBag.MinConsumptionDropDown = SetConsumptionDropDown(MinOrMax.Min, consumptionFrom);
            ViewBag.MaxConsumptionDropDown = SetConsumptionDropDown(MinOrMax.Max, consumptionTo);
            ViewBag.MinPricesDropDown = SetPricesDropDown(MinOrMax.Min, priceFrom);
            ViewBag.MaxPricesDropDown = SetPricesDropDown(MinOrMax.Max, priceTo);
            return View(modelList);
        }
           
        // todo: Secure that selected vehicle wasn't taken in the meantime        
        [HttpGet]
        [Route("Home/VehicleSelection/{vehicleId}/Payment")]
        public ActionResult Payment(int vehicleId)
        {
            Order order = (Order)Session[OrderSession];

            if (order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            Vehicle vehicle = _db.Vehicles.Find(vehicleId);

            if (vehicle == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            order.UserId = User.Identity.GetUserId<int>();

            order.VehicleId = vehicleId;
            order.Vehicle = vehicle;

            PaymentTypeViewModel model = new PaymentTypeViewModel();

            var creditCardData = _dbUser.CreditCards
                .Where(c => c.UserId == order.UserId && c.Status != EntityStatus.Archived)
                .FirstOrDefault();

            if (creditCardData != null)
            {
                model.CardNumber = creditCardData.CardNumber;
                model.ExpirationDate = creditCardData.ExpirationDate;
                model.PaymentTypeId = creditCardData.PaymentTypeId;
            }

            ViewBag.vehicleId = vehicleId; // For url
            ViewBag.PaymentTypesDropDown = SetPaymentTypesDropDown(model.PaymentTypeId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Home/VehicleSelection/{vehicleId}/Payment")]
        public ActionResult Payment(int vehicleId, PaymentTypeViewModel model)
        {
            try
            {
                var payments = _dbUser.PaymentTypes
                    .Where(p => p.Status != EntityStatus.Archived)
                    .ToList();

                bool validPayment;
                Order order = (Order)Session[OrderSession];
                if (order == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }

                if (ModelState.IsValid)
                {
                    if (model.ExpirationDate.Date < DateTime.Now.Date) throw new InvalidOperationException(String.Format("Expiration date {0:dd.MM.yyyy} must be higher than today's date {1:dd.MM.yyyy}.", model.ExpirationDate.ToShortDateString(), DateTime.Now.ToShortDateString()));

                    // todo: When paygate service is enabled, uncomment this and send proper data
                    // validPayment = isTransactionValid(model, order);

                    validPayment = true;

                    if (validPayment)
                    {
                        CreditCard creditCard = new CreditCard()
                        {
                            PaymentTypeId = model.PaymentTypeId,
                            UserId = order.UserId,
                            ExpirationDate = model.ExpirationDate,
                            OwnerFirstName = model.OwnerFirstName,
                            OwnerLastName = model.OwnerLastName,
                            CardNumber = model.CardNumber
                        };

                        var dbCard = GetExistingCreditCard(creditCard, order.UserId);

                        if (dbCard == null)
                        {
                            _dbUser.CreditCards.Add(creditCard);
                            _dbUser.SaveChanges();
                        }
                        else
                        {
                            creditCard.ID = dbCard.ID;
                        }

                        order.CreditCardId = creditCard.ID;
                        order.PaymentTypeId = creditCard.PaymentTypeId;

                        return RedirectToAction("OrderSummary", new { vehicleId = vehicleId });
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Payment is not valid. Please try again.");
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
            }
            catch
            {
                ModelState.AddModelError(String.Empty, "Unable to process payment.");
            }

            ViewBag.vehicleId = vehicleId; // For url
            ViewBag.PaymentTypesDropDown = SetPaymentTypesDropDown(model.PaymentTypeId);

            return View(model);
        }

        [HttpGet]
        [Route("Home/VehicleSelection/{vehicleId}/Payment/OrderSummary")]
        public ActionResult OrderSummary(int vehicleId)
        {
            Order order = (Order)Session[OrderSession];

            if (order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            OrderSummaryViewModel model = new OrderSummaryViewModel();

            model.Order = order;

            // Convert locations pictures to base64 string
            if (order.ReceiveLocation.LocationPicture != null)
            {
                model.ReceiveLocationImageBase64 = String.Format("data:image;base64,{0}", Convert.ToBase64String(order.ReceiveLocation.LocationPicture));
            }
            if (order.DestinationLocation.LocationPicture != null)
            {
                model.DestinationLocationImageBase64 = String.Format("data:image;base64,{0}", Convert.ToBase64String(order.DestinationLocation.LocationPicture));
            }

            model.User = _dbUser.Users.SingleOrDefault(u => u.Id == order.UserId);

            if (model.User == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            model.VehicleModel = SetVehicleSelectionViewModel(order.Vehicle, order);
            order.TotalPrice = model.VehicleModel.TotalPriceForPeriod;

            ViewBag.vehicleId = vehicleId;
            ViewBag.ReceiveDate = order.RecvTime;
            ViewBag.DestinationDate = order.DestTime;

            return View(model);
        }

        [HttpPost]
        public ActionResult OrderSummary()
        {
            string returnMsg = "";

            try
            {
                Order order = (Order)Session[OrderSession];

                if (order != null)
                {
                    Order newOrder = new Order()
                    {
                        OrderNumber = order.OrderNumber,
                        VehicleId = order.VehicleId,
                        UserId = order.UserId,
                        CreditCardId = order.CreditCardId,
                        RecvLocationId = order.RecvLocationId,
                        RecvTime = order.RecvTime,
                        DestLocationId = order.DestLocationId,
                        DestTime = order.DestTime,
                        TotalPrice = order.TotalPrice,
                        PaymentTypeId = order.PaymentTypeId,
                        Status = EntityStatus.Active
                    };

                    Vehicle reservedVehicle = _db.Vehicles
                        .Single(v => v.ID == order.VehicleId);

                    reservedVehicle.Status = EntityStatus.Active;
                    reservedVehicle.Free = Available.Taken;
                    _db.Entry(reservedVehicle).State = System.Data.Entity.EntityState.Modified;

                    _db.Orders.Add(newOrder);
                    int result = _db.SaveChanges();

                    if (result > -1)
                    {
                        OrderSummaryViewModel model = new OrderSummaryViewModel()
                        {
                            Order = order,
                            VehicleModel = SetVehicleSelectionViewModel(reservedVehicle, order),
                            User = _dbUser.Users.SingleOrDefault(u => u.Id == order.UserId)
                        };

                        AppEmail appEmail = new AppEmail();

                        if (appEmail.SendOrder(model) > 0)
                        {
                            TempData["orderSuccess"] = new string[]
                            {
                                String.Format("Order is confirmed and has been sent to e-mail address: {0}", User.Identity.GetUserName()),
                                "Thank you for using our services."
                            };

                            //returnMsg = String.Format("Order is confirmed and has been sent on mail address: {0}!", User.Identity.GetUserName());
                            returnMsg = "Success";
                            return Json(new { headMsg = returnMsg, bodyMsg = "Thank you for using our services." }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                returnMsg = "Error confirming order, please try again.";

            }
            catch (NullReferenceException e)
            {
                returnMsg = "Error confirming order, please try again.";
            }
            catch
            {
                returnMsg = "Error confirming order, please try again.";
            }

            return new JsonBadRequest(new { errorHead = returnMsg, errorBody = "Something went wrong." });
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult Places(string selectedCity)
        {
            if (!String.IsNullOrEmpty(selectedCity))
            {
                var places = _db.VehicleLocation
                    .Where(loc => loc.City.ToLower() == selectedCity.ToLower() && loc.Status != EntityStatus.Archived)
                    .OrderBy(loc => loc.Place)
                    .Select(loc => loc.Place)
                    .ToArray<string>();

                if (places != null)
                {
                    return this.Json(places, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "There are no places for selected city. Please select another one.");
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Error getting selected city.");
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        private SelectList SetVehicleTypeDropDown(object selectedValue = null)
        {
            List<string> vehicleTypes = AppValues.VehicleTypes.OrderBy(t => t).Distinct().ToList();

            vehicleTypes.Insert(0, AppValues.ALL);
            return new SelectList(vehicleTypes, selectedValue);
        }

        private SelectList SetBrandDropDown(object selectedValue = null)
        {
            List<string> brands = _db.Vehicles.Select(v => v.Brand).OrderBy(t => t).Distinct().ToList();

            brands.Insert(0, AppValues.ALL);
            return new SelectList(brands, selectedValue);
        }

        private enum MinOrMax { Min, Max };

        private SelectList SetConsumptionDropDown(MinOrMax valueType, object selectedValue = null)
        {
            // todo: Fix so that no communication with db is established for this
            int min = _db.Vehicles.Select(v => (int)v.GasPerKm).Min();
            int max = _db.Vehicles.Select(v => (int)v.GasPerKm).Max();

            List<int> consumption = new List<int>();

            for (int i = min; i <= max; i++)
            {
                consumption.Add(i);
            }

            switch (valueType)
            {
                case MinOrMax.Min:
                    if ((int)selectedValue == default(int))
                    {
                        selectedValue = min;
                    }
                    break;

                case MinOrMax.Max:
                    if ((int)selectedValue == int.MaxValue)
                    {
                        selectedValue = max;
                    }

                    break;
            }

            return new SelectList(consumption, selectedValue);
        }

        private SelectList SetPricesDropDown(MinOrMax valueType, object selectedValue = null)
        {
            // todo: Fix so that no query is executed
            int min = _db.Vehicles.Select(v => (int)v.DayPrice).Min();
            int max = _db.Vehicles.Select(v => (int)v.DayPrice).Max();

            List<int> prices = new List<int>();

            for (int i = min; i <= max; i++)
            {
                prices.Add(i);
            }

            switch (valueType)
            {
                case MinOrMax.Min:
                    if ((int)selectedValue == default(int))
                    {
                        selectedValue = min;
                    }
                    break;

                case MinOrMax.Max:
                    if ((int)selectedValue == int.MaxValue)
                    {
                        selectedValue = max;
                    }

                    break;
            }

            return new SelectList(prices, selectedValue);
        }


        private VehicleSelectionViewModel SetVehicleSelectionViewModel(Vehicle vehicle, Order order)
        {
            var carsEquipment = _db.CarEquipment
                .Where(item => vehicle.ID == item.VehicleID && item.Status != EntityStatus.Archived)
                .ToList();

            int daysPeriod = order.DestTime.DayOfYear - order.RecvTime.DayOfYear;

            if (daysPeriod <= 0)
            {
                daysPeriod = 1;
            }

            VehicleSelectionViewModel model = new VehicleSelectionViewModel()
            {
                ID = vehicle.ID,
                Brand = vehicle.Brand,
                BrandType = vehicle.BrandType,
                DoorsCount = vehicle.DoorsCount,
                GasPerKm = vehicle.GasPerKm,
                HorsePower = vehicle.HorsePower,
                SeatCount = vehicle.SeatCount,
                Mileage = vehicle.Mileage.HasValue ? vehicle.Mileage.Value : 0,
                Airbags = vehicle.Airbags,
                VehicleType = vehicle.VehicleType,
                ModelYear = vehicle.ModelYear,
                DayPrice = vehicle.DayPrice,
                ImagesBase64 = vehicle.Picture != null ? String.Format("data:image;base64,{0}", Convert.ToBase64String(vehicle.Picture)) : null,
                AdditionalEquipment = carsEquipment.Where(x => x.VehicleID == vehicle.ID).Select(x => x.Equipment).ToList()
            };

            model.Fuel = _db.Fuel.SingleOrDefault(f => f.ID == vehicle.FuelId).FuelName;

            model.TotalPriceForPeriod = (vehicle.DayPrice * daysPeriod) + model.AdditionalEquipment.Sum(x => x.Price);

            return model;
        }

        private VehicleSelectionViewModel SetVehicleSelectionViewModel(Vehicle vehicle, Order order, HashSet<int> vehicleListIdsHS, List<CarEquipment> carsEquipment)
        {
            int daysPeriod = order.DestTime.DayOfYear - order.RecvTime.DayOfYear;

            if (daysPeriod <= 0)
            {
                daysPeriod = 1;
            }

            VehicleSelectionViewModel model = new VehicleSelectionViewModel()
            {
                ID = vehicle.ID,
                Brand = vehicle.Brand,
                BrandType = vehicle.BrandType,
                DoorsCount = vehicle.DoorsCount,
                GasPerKm = vehicle.GasPerKm,
                HorsePower = vehicle.HorsePower,
                SeatCount = vehicle.SeatCount,
                Mileage = vehicle.Mileage.HasValue ? vehicle.Mileage.Value : 0,
                Airbags = vehicle.Airbags,
                VehicleType = vehicle.VehicleType,
                ModelYear = vehicle.ModelYear,
                DayPrice = vehicle.DayPrice,
                ImagesBase64 = vehicle.Picture != null ? String.Format("data:image;base64,{0}", Convert.ToBase64String(vehicle.Picture)) : null,
                AdditionalEquipment = carsEquipment.Where(x => x.VehicleID == vehicle.ID).Select(x => x.Equipment).ToList()
            };

            model.TotalPriceForPeriod = (vehicle.DayPrice * daysPeriod) + model.AdditionalEquipment.Sum(x => x.Price);

            return model;
        }

        private CreditCard GetExistingCreditCard(CreditCard card, int userId)
        {
            var dbCreditCard = _dbUser.CreditCards
                .Where(c => c.CardNumber == card.CardNumber && c.ExpirationDate == card.ExpirationDate && c.PaymentTypeId == card.PaymentTypeId && c.UserId == userId)
                .FirstOrDefault();

            return dbCreditCard;
        }

        private SelectList SetPaymentTypesDropDown(object selectedValue = null)
        {
            var paymentTypes = _dbUser.PaymentTypes
                .Where(p => p.Status != EntityStatus.Archived)
                .ToList();

            return new SelectList(paymentTypes, "ID", "Name", selectedValue);
        }

        private SelectList SetLocationPlaceDropDown(List<VehicleLocation> locationList, string filterCity)
        {
            string[] places = locationList.Where(loc => loc.City.ToLower() == filterCity.ToLower())
                .OrderBy(loc => loc.Place)
                .Select(loc => loc.Place)
                .ToArray();

            return new SelectList(places);
        }

        private SelectList SetLocationCitiesDropDown(List<VehicleLocation> locationList, object selectedValue = null)
        {
            string[] cities = locationList.OrderBy(loc => loc.City)
                .Select(loc => loc.City)
                .Distinct()
                .ToArray();

            return new SelectList(cities, selectedValue);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RentACar.DAT;
using RentACar.Models;
using RentACar.App_Helpers;
using RentACar.ViewModels;

namespace RentACar.Controllers
{
    [Authorize(Roles = "Admin, SuperEmployee, Employee")]
    public class VehiclesController : Controller
    {
        private RcDbContext _db = new RcDbContext();

        // GET: Vehicles
        public ActionResult Index(int[] vehicleStatus)
        {

            if (vehicleStatus == null || vehicleStatus.Length < 1)
            {
                vehicleStatus = new int[] { (int)EntityStatus.Active, (int)EntityStatus.New };
            }

            var vehicles =
                _db.Vehicles
                .Where(v => vehicleStatus.Contains((int)v.Status))
                .OrderBy(v => v.Brand)
                .ToList();

            ViewBag.statusList = new EntityStatusView[]
            {
                new EntityStatusView { StatusName = AppValues.EntityStatus.ACTIVE, StatusValue = (int) EntityStatus.Active, Checked = vehicleStatus.Contains((int) EntityStatus.Active) ? true : false },
                new EntityStatusView { StatusName = AppValues.EntityStatus.NEW, StatusValue = (int) EntityStatus.New, Checked = vehicleStatus.Contains((int) EntityStatus.New) ? true : false },
                new EntityStatusView { StatusName = AppValues.EntityStatus.ARCHIVED, StatusValue = (int) EntityStatus.Archived, Checked = vehicleStatus.Contains((int) EntityStatus.Archived) ? true : false }
            };

            return View(vehicles);
        }

        // GET: Vehicles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleViewModel model = new VehicleViewModel();
            model.Vehicle = _db.Vehicles.Find(id);
            if (model.Vehicle == null)
            {
                return HttpNotFound();
            }

            List<Equipment> equipment = _db.Equipment
                .ToList();

            AppValues.ImageViewModel imageView;

            if (model.Vehicle.Picture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(model.Vehicle.Picture)), null, null);
                model.ImagesBase64.Add(imageView);
            }

            model.AdditionalEquipment = GetAssignedEquipmentSetup(model);

            return View(model);
        }

        // GET: Vehicles/Create
        public ActionResult Create()
        {
            VehicleViewModel model = new VehicleViewModel();
            List<Equipment> equipment = _db.Equipment
                .ToList();

            model.AdditionalEquipment = GetAdditionalEquipment(model);

            ViewBag.ModelYearDropDown = SetDropDownModelYear();
            ViewBag.VehicleTypeDropDown = SetVehicleTypeDropDown();
            ViewBag.FuelDropdown = SetFuelTypeDropDown();
            return View(model);
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VehicleViewModel model, int[] selectedEquipment, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Vehicle vehicle = new Vehicle();

                    vehicle.VehicleType = model.Vehicle.VehicleType;
                    vehicle.Brand = model.Vehicle.Brand;
                    vehicle.BrandType = model.Vehicle.BrandType;
                    vehicle.GasPerKm = model.Vehicle.GasPerKm;
                    vehicle.HorsePower = model.Vehicle.HorsePower;
                    vehicle.DoorsCount = model.Vehicle.DoorsCount;
                    vehicle.Registration = model.Vehicle.Registration;
                    vehicle.RegistrationExpire = model.Vehicle.RegistrationExpire;
                    vehicle.SeatCount = model.Vehicle.SeatCount;
                    vehicle.Mileage = model.Vehicle.Mileage;
                    vehicle.DayPrice = model.Vehicle.DayPrice;
                    vehicle.AcquiredDate = model.Vehicle.AcquiredDate;
                    vehicle.Airbags = model.Vehicle.Airbags;                    
                    vehicle.ModelYear = model.Vehicle.ModelYear;
                    vehicle.FuelId = model.Vehicle.FuelId;

                    if (IsRegistrationPlateUnique(vehicle))
                    {
                        if (model.Files != null)
                        {
                            vehicle.Picture = AppMethods.GetPicture(model.Files[0]);
                            AppMethods.AddPhotoInDb(_db, new Photos() { Content = vehicle.Picture, ItemID = vehicle.ID, ItemType = AppValues.PhotoParameters.LOCATION }, false);
                        }
                                                
                        vehicle.Free = Available.Free;
                        _db.Vehicles.Add(vehicle);

                        SetAdditionalEquipment(vehicle, selectedEquipment);
                        _db.SaveChanges();
                        return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, String.Format("Registration plate {0} is not unique!", vehicle.Registration));
                    }
                }
            }
            catch
            {
                ModelState.AddModelError(String.Empty, AppValues.Messages.CREATEFAILED);
            }
                      

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        // GET: Vehicles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleViewModel model = new VehicleViewModel();

            model.Vehicle = _db.Vehicles.Find(id.Value);
            if (model.Vehicle == null)
            {
                return HttpNotFound();
            }
            
            AppValues.ImageViewModel imageView;

            if (model.Vehicle.Picture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(model.Vehicle.Picture)), null, null);
                model.ImagesBase64.Add(imageView);
            }

            model.AdditionalEquipment = GetAdditionalEquipment(model);
            ViewBag.ModelYearDropDown = SetDropDownModelYear(model.Vehicle.ModelYear);
            ViewBag.VehicleTypeDropDown = SetVehicleTypeDropDown(model.Vehicle.VehicleType);
            ViewBag.FuelDropdown = SetFuelTypeDropDown(model.Vehicle.Fuel);
            return View(model);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VehicleViewModel model, int[] selectedEquipment, FormCollection collection)
        {
            Vehicle vehicle = _db.Vehicles.Find(id);

            if (vehicle == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    vehicle.VehicleType = model.Vehicle.VehicleType;
                    vehicle.Brand = model.Vehicle.Brand;
                    vehicle.BrandType = model.Vehicle.BrandType;
                    vehicle.GasPerKm = model.Vehicle.GasPerKm;
                    vehicle.HorsePower = model.Vehicle.HorsePower;
                    vehicle.DoorsCount = model.Vehicle.DoorsCount;
                    vehicle.Registration = model.Vehicle.Registration;
                    vehicle.RegistrationExpire = model.Vehicle.RegistrationExpire;
                    vehicle.SeatCount = model.Vehicle.SeatCount;
                    vehicle.Mileage = model.Vehicle.Mileage;
                    vehicle.DayPrice = model.Vehicle.DayPrice;
                    vehicle.AcquiredDate = model.Vehicle.AcquiredDate;
                    vehicle.Airbags = model.Vehicle.Airbags;
                    vehicle.ModelYear = model.Vehicle.ModelYear;
                    vehicle.FuelId = model.Vehicle.FuelId;

                    if (IsRegistrationPlateUnique(vehicle))
                    {
                        if (model.Files != null)
                        {
                            byte[] newPicture = AppMethods.GetPicture(model.Files[0]);
                            byte[] oldPicture = vehicle.Picture ?? new byte[0];

                            if (newPicture != null)
                            {
                                if (!newPicture.SequenceEqual(oldPicture))
                                {
                                    vehicle.Picture = newPicture;
                                    AppMethods.AddPhotoInDb(_db, new Photos() { Content = vehicle.Picture, ItemID = vehicle.ID, ItemType = AppValues.PhotoParameters.LOCATION }, false);
                                }
                            }
                        }

                        SetAdditionalEquipment(vehicle, selectedEquipment);
                        _db.Entry(vehicle).State = EntityState.Modified;
                        _db.SaveChanges();
                        return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, String.Format("Registration plate {0} is not unique!", vehicle.Registration));
                    }
                }               
            }
            catch
            {

                ModelState.AddModelError(String.Empty, AppValues.Messages.EDITFAILED);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        // GET: Vehicles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleViewModel model = new VehicleViewModel();
            model.Vehicle = _db.Vehicles.Find(id);
            if (model.Vehicle == null)
            {
                return HttpNotFound();
            }

            List<Equipment> equipment = _db.Equipment
                .ToList();

            AppValues.ImageViewModel imageView;

            if (model.Vehicle.Picture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(model.Vehicle.Picture)), null, null);
                model.ImagesBase64.Add(imageView);
            }

            model.AdditionalEquipment = GetAssignedEquipmentSetup(model);

            return View(model);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Vehicle vehicle = _db.Vehicles.Find(id);

                vehicle.Status = EntityStatus.Archived;

                ArchiveCarEquipmentRelation(vehicle);

                _db.SaveChanges();
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                ModelState.AddModelError(String.Empty, AppValues.Messages.DELETEFAILED);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });

        }

        private int ArchiveCarEquipmentRelation(Vehicle vehicle)
        {
            var carEquipment =
                (
                from entity in _db.CarEquipment
                where entity.VehicleID == vehicle.ID && entity.Status != EntityStatus.Archived
                select entity
                ).ToList();

            if (carEquipment != null)
            {
                foreach (var item in carEquipment)
                {
                    item.Status = EntityStatus.Archived;
                }
            }

            return 1;
        }

        private List<VehicleViewModel.AssignedEquipment> GetAssignedEquipmentSetup(VehicleViewModel model)
        {
            var assignedEquipment =
                (
                from entity in _db.CarEquipment
                where entity.VehicleID == model.Vehicle.ID && entity.Status != EntityStatus.Archived
                select entity.Equipment
                ).ToList();

            foreach (var equipment in assignedEquipment)
            {
                model.AdditionalEquipment.Add(
                    new VehicleViewModel.AssignedEquipment() { Equipment = equipment, Assigned = true });
            }

            return model.AdditionalEquipment;
        }

        private List<VehicleViewModel.AssignedEquipment> GetAdditionalEquipment(VehicleViewModel model)
        {
            List<Equipment> allEquipment = _db.Equipment
                .Where(e => e.Status != EntityStatus.Archived)
                .ToList();

            var carEquipment =
                (
                from entity in _db.CarEquipment
                where entity.VehicleID == model.Vehicle.ID && entity.Status != EntityStatus.Archived
                select entity.Equipment
                ).ToList();

            foreach (var equipment in allEquipment)
            {
                model.AdditionalEquipment.Add(
                    new VehicleViewModel.AssignedEquipment() { Equipment = equipment, Assigned = carEquipment.Contains(equipment) ? true : false });
            }

            return model.AdditionalEquipment;
        }

        private int SetAdditionalEquipment(Vehicle vehicle, int[] selectedEquipment)
        {
            var carEquipmentList = _db.CarEquipment
                .Where(entity => entity.VehicleID == vehicle.ID && entity.Status != EntityStatus.Archived)
                .ToList();

            HashSet<int> carEquipmentIDsHS = new HashSet<int>(carEquipmentList.Select(item => item.EquipmentID));

            foreach (var item in carEquipmentList)
            {
                if (selectedEquipment == null || selectedEquipment.Length < 1)
                {
                    item.Status = EntityStatus.Archived;
                }
                else if (!selectedEquipment.Contains(item.EquipmentID))
                {
                    item.Status = EntityStatus.Archived;
                }
            }

            if (selectedEquipment != null)
            {
                foreach (var item in selectedEquipment)
                {
                    if (!carEquipmentIDsHS.Contains(item))
                    {
                        _db.CarEquipment.Add(new CarEquipment() { VehicleID = vehicle.ID, EquipmentID = item });
                    }
                }
            }


            return 1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private SelectList SetVehicleTypeDropDown(object selectedValue = null)
        {
            return new SelectList(AppValues.VehicleTypes, selectedValue);
        }

        private SelectList SetDropDownModelYear(object selectedValue = null)
        {
            int currentYear = DateTime.Now.Year;

            List<string> years = new List<string>();

            for (int i = AppValues.MINMODELYEAR; i < currentYear; i++)
            {
                years.Add(i.ToString());
            }

            return new SelectList(years, selectedValue);
        } 

        private bool IsRegistrationPlateUnique(Vehicle vehicle)
        {
            if (vehicle == null) { return false; }

            var countSamePlates = _db.Vehicles
                .Where(v => v.ID != vehicle.ID && v.Registration == vehicle.Registration && v.Status != EntityStatus.Archived)
                .Count();

            return countSamePlates > 0 ? false : true;
        }

        private SelectList SetFuelTypeDropDown(object selectedFuel = null)
        {
            var fuels = _db.Fuel
                .Where(f => f.Status != EntityStatus.Archived)
                .ToList();

            return new SelectList(fuels, "ID", "FuelName", selectedFuel);
        }
    }
}

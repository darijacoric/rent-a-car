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
    public class VehicleLocationsController : Controller
    {
        private RcDbContext _db = new RcDbContext();

        // GET: VehicleLocations
        public ActionResult Index()
        {
            var vehicleLocationList = _db.VehicleLocation
                .Where(item => item.Status != EntityStatus.Archived)
                .ToList();

            return View(vehicleLocationList);
        }

        // GET: VehicleLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VehicleLocationViewModel model = new VehicleLocationViewModel();

            model.VehicleLocation = _db.VehicleLocation.Find(id);
            if (model.VehicleLocation == null)
            {
                return HttpNotFound();
            }

            AppValues.ImageViewModel imageView;

            if (model.VehicleLocation.LocationPicture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(model.VehicleLocation.LocationPicture)), null, null);
                model.ImagesBase64.Add(imageView);
            }

            return View(model);
        }

        // GET: VehicleLocations/Create
        public ActionResult Create()
        {
            return View();
        }

        // todo: Dodati provjeru ukoliko dodano mjesto već postoji u bazi
        // POST: VehicleLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VehicleLocationViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    VehicleLocation vehicleLocation = new VehicleLocation();

                    vehicleLocation.City = model.VehicleLocation.City;
                    vehicleLocation.Place = model.VehicleLocation.Place;

                    if (IsNewPlaceUnique(vehicleLocation))
                    {
                        vehicleLocation.Description = model.VehicleLocation.Description;

                        if (model.Files != null)
                        {
                            vehicleLocation.LocationPicture = AppMethods.GetPicture(model.Files[0]);
                            AppMethods.AddPhotoInDb(_db, new Photos() { Content = vehicleLocation.LocationPicture, ItemID = vehicleLocation.ID, ItemType = AppValues.PhotoParameters.LOCATION }, false);
                        }

                        _db.VehicleLocation.Add(vehicleLocation);
                        _db.SaveChanges();
                        return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, String.Format("{0} already exists in database.", vehicleLocation.DisplayLocation));
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Location data is not valid.");
                }
            }
            catch
            {
                ModelState.AddModelError(String.Empty, AppValues.Messages.EDITFAILED);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        private bool IsNewPlaceUnique(VehicleLocation newLocation)
        {
            int count =
                _db.VehicleLocation
                .Where(loc => loc.City.ToLower() == newLocation.City.ToLower() && loc.Place.ToLower() == newLocation.Place.ToLower() && loc.Status != EntityStatus.Archived)
                .Count();

            return count > 0 ? false : true;
        }

        // GET: VehicleLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VehicleLocationViewModel model = new VehicleLocationViewModel();

            model.VehicleLocation = _db.VehicleLocation.Find(id);
            if (model.VehicleLocation == null)
            {
                return HttpNotFound();
            }

            AppValues.ImageViewModel imageView;

            if (model.VehicleLocation.LocationPicture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(model.VehicleLocation.LocationPicture)), null, null);
                model.ImagesBase64.Add(imageView);
            }

            return View(model);
        }

        // POST: VehicleLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VehicleLocationViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {

                if (ModelState.IsValid)
                {
                    // For some reason, model.VehicleLocation doesn't have ID despite it had ID in HttpGet Edit Action
                    VehicleLocation vehicleLocation = _db.VehicleLocation.Find(id);

                    if (vehicleLocation == null) { return new HttpStatusCodeResult(HttpStatusCode.InternalServerError); }

                    vehicleLocation.City = model.VehicleLocation.City;
                    vehicleLocation.Place = model.VehicleLocation.Place;
                    vehicleLocation.Description = model.VehicleLocation.Description;

                    if (model.Files != null)
                    {
                        byte[] newPicture = AppMethods.GetPicture(model.Files[0]);
                        byte[] oldPicture = vehicleLocation.LocationPicture ?? new byte[0];

                        if (newPicture != null)
                        {
                            if (!newPicture.SequenceEqual(oldPicture))
                            {
                                vehicleLocation.LocationPicture = newPicture;
                                AppMethods.AddPhotoInDb(_db, new Photos() { Content = vehicleLocation.LocationPicture, ItemID = vehicleLocation.ID, ItemType = AppValues.PhotoParameters.LOCATION }, false);
                            }
                        }
                    }

                    _db.Entry(vehicleLocation).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }

                ModelState.AddModelError(String.Empty, AppValues.Messages.EDITFAILED);
            }
            catch
            {
                ModelState.AddModelError(String.Empty, AppValues.Messages.EDITFAILED);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        // GET: VehicleLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VehicleLocationViewModel model = new VehicleLocationViewModel();

            model.VehicleLocation = _db.VehicleLocation.Find(id);
            if (model.VehicleLocation == null)
            {
                return HttpNotFound();
            }

            AppValues.ImageViewModel imageView;

            if (model.VehicleLocation.LocationPicture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(model.VehicleLocation.LocationPicture)), null, null);
                model.ImagesBase64.Add(imageView);
            }

            return View(model);
        }

        // POST: VehicleLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                VehicleLocation vehicleLocation = _db.VehicleLocation.Find(id);

                if (vehicleLocation == null) throw new ArgumentNullException(AppValues.Messages.DELETEFAILED);

                vehicleLocation.Status = EntityStatus.Archived;

                _db.SaveChanges();
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentNullException e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

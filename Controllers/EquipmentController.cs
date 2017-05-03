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

namespace RentACar.Controllers
{
    [Authorize(Roles = "Admin, SuperEmployee, Employee")]
    public class EquipmentController : Controller
    {
        private RcDbContext _db = new RcDbContext();

        // GET: Equipment
        public ActionResult Index()
        {
            var equipment = _db.Equipment
                .Where(e => e.Status != EntityStatus.Archived)
                .ToList();

            return View(equipment);
        }

        // GET: Equipment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = _db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // GET: Equipment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Equipment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Price,Description")] Equipment equipment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Equipment.Add(equipment);
                    _db.SaveChanges();
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                ModelState.AddModelError(String.Empty, AppValues.Messages.EDITFAILED);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        // GET: Equipment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = _db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            string[] bindProperties = new string[] { "Name", "Price", "Description" };

            var equipment = _db.Equipment.SingleOrDefault(e => e.ID == id);

            if (equipment == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                if (TryUpdateModel(equipment, bindProperties))
                {
                    _db.Entry(equipment).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Invalid data.");
                }
            }
            catch
            {
                ModelState.AddModelError(String.Empty, AppValues.Messages.EDITFAILED);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        // GET: Equipment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = _db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Equipment equipment = _db.Equipment.Find(id);

                if (equipment == null) throw new ArgumentNullException(AppValues.Messages.DELETEFAILED);

                equipment.Status = EntityStatus.Archived;
                ArchiveCarEquipmentRelation(equipment);

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

        private int ArchiveCarEquipmentRelation(Equipment equipment)
        {
            var carEquipment =
                (
                from entity in _db.CarEquipment
                where entity.EquipmentID == equipment.ID && entity.Status != EntityStatus.Archived
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

    }
}

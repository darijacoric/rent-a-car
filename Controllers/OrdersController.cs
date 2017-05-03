using RentACar.App_Helpers;
using RentACar.DAT;
using RentACar.Models;
using RentACar.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RentACar.Controllers
{
    [Authorize(Roles = "Admin, SuperEmployee, Employee")]
    public class OrdersController : Controller
    {
        RcDbContext _db = new RcDbContext();
        UserDbContext _dbUser = new UserDbContext();


        public ActionResult Index(int[] orderStatus = null)
        {
            if (orderStatus == null)
            {
                orderStatus = new int[] { (int)EntityStatus.Active, (int)EntityStatus.Archived };
            }

            var orders = _db.Orders
                .Where(o => orderStatus.Contains((int)o.Status))
                .Include(o => o.Vehicle)
                .Include(o => o.ReceiveLocation)
                .Include(o => o.DestinationLocation)
                .ToList();

            var usersFromOrdersHS = new HashSet<int>(orders.Select(o => o.UserId));
            var paymentTypesFromOrdersHS = new HashSet<int>(orders.Select(o => o.PaymentTypeId));

            var users = _dbUser.Users
                .Where(u => usersFromOrdersHS.Contains(u.Id))
                .Include(u => u.CreditCards)
                .ToList();

            var payments = _dbUser.PaymentTypes
                .Where(p => paymentTypesFromOrdersHS.Contains(p.ID))
                .ToList();

            var query =
                from order in orders
                join user in users on order.UserId equals user.Id
                join payment in payments on order.PaymentTypeId equals payment.ID
                select new
                {
                    Order = order,
                    User = user,
                    CreditCard = user.CreditCards.Where(c => c.ID == order.CreditCardId).SingleOrDefault(),
                    PaymentType = payment
                };

            List<OrderViewModel> models = new List<OrderViewModel>();

            foreach (var item in query)
            {
                models.Add(new OrderViewModel()
                {
                    Order = item.Order,
                    User = item.User,
                    CreditCard = item.CreditCard,
                    PaymentType = item.PaymentType
                });
            }

            ViewBag.statusList = new EntityStatusView[]
           {
                new EntityStatusView { StatusName = AppValues.EntityStatus.ACTIVE, StatusValue = (int) EntityStatus.Active, Checked = orderStatus.Contains((int) EntityStatus.Active) ? true : false },
                new EntityStatusView { StatusName = AppValues.EntityStatus.ARCHIVED, StatusValue = (int) EntityStatus.Archived, Checked = orderStatus.Contains((int) EntityStatus.Archived) ? true : false }
           };

            return View(models);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            string errorMsg = null;
            try
            {
                Order order = _db.Orders
                .Where(o => o.ID == id)
                .Include(o => o.Vehicle)
                .Include(o => o.ReceiveLocation)
                .Include(o => o.DestinationLocation)
                .SingleOrDefault();

                if (order == null)
                {
                    throw new NullReferenceException("Invalid Order!");
                }

                var model = SetOrderViewModel(order);

                return View(model);
            }
            catch (NullReferenceException e)
            {
                errorMsg = e.Message;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errorMsg);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            string errorMsg = null;
            try
            {
                Order order = _db.Orders
                .Where(o => o.ID == id)
                .Include(o => o.Vehicle)
                .Include(o => o.ReceiveLocation)
                .Include(o => o.DestinationLocation)
                .SingleOrDefault();

                if (order == null)
                {
                    throw new NullReferenceException("Invalid Order!");
                }

                var model = SetOrderViewModel(order);

                return View(model);
            }
            catch (NullReferenceException e)
            {
                errorMsg = e.Message;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errorMsg);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]        
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Order order = _db.Orders
                   .Where(o => o.ID == id)
                   .Include(o => o.Vehicle)
                   .SingleOrDefault();

                if (order != null && order.Vehicle != null)
                {
                    order.Status = EntityStatus.Archived;
                    _db.Entry<Order>(order).State = EntityState.Modified;

                    order.Vehicle.Status = EntityStatus.New;
                    order.Vehicle.Free = Available.Free;
                    _db.Entry<Vehicle>(order.Vehicle).State = EntityState.Modified;

                    _db.SaveChanges();
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }

                ModelState.AddModelError(String.Empty, "Invalid Order!");
            }
            catch
            {
                ModelState.AddModelError(String.Empty, AppValues.Messages.DELETEFAILED);
            }

            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        private OrderViewModel SetOrderViewModel(Order order)
        {
            var user = _dbUser.Users
                .Where(u => u.Id == order.UserId)
                .Include(u => u.CreditCards)
                .SingleOrDefault();

            if (user == null)
            {
                throw new NullReferenceException("Invalid user data!");
            }

            var paymentType = _dbUser.PaymentTypes
                .Where(p => p.ID == order.PaymentTypeId)
                .SingleOrDefault();


            if (user == null)
            {
                throw new NullReferenceException("Invalid payment data!");
            }

            return new OrderViewModel()
            {
                Order = order,
                User = user,
                CreditCard = user.CreditCards.Where(c => c.ID == order.CreditCardId).SingleOrDefault(),
                PaymentType = paymentType
            };
        }
    }
}
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RentACar.App_Helpers;
using RentACar.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RentACar.Controllers
{   
    // todo: Add employee model which will have hired date, fired date (if fired) etc. to separate from AppUser. Inherit Employee model from AppUser
     
    [Authorize(Roles = "Admin, SuperEmployee" )]
    public class EmployeeController : Controller
    {
        private UserDbContext _db = new UserDbContext();

        CustomRoleStore _roleStore;
        CustomUserStore _userStore;
        CustomUserManager _userManager;
        CustomRoleManager _roleManager;

        public EmployeeController()
        {
            _roleStore = new CustomRoleStore(_db);
            _roleManager = new CustomRoleManager(_roleStore);
            _userStore = new CustomUserStore(_db);
            _userManager = new CustomUserManager(_userStore);
        }
        
        // GET: Employee
        public ActionResult Index()
        {
            List<int> wantedRolesId = _db.Roles
                .Where(r => r.Name != AppValues.AppRoles.USER && r.Name != AppValues.AppRoles.ADMIN)
                .Select(r => r.Id)
                .ToList();

            var ids = String.Join(",", wantedRolesId);
            
            string sqlQuery = @"Select *
                                from dbo.Users as users
                                inner join dbo.UserRoles as userRoles on users.Id = userRoles.UserId
                                where userRoles.RoleId in (" + ids + ") and " +
                                "users.AccountStatus = @userStatus;";
            Object[] parameters = new Object[]
            {
                new SqlParameter("@userStatus", UserStatus.Enabled)
            };

            var users = _db.Database.SqlQuery<AppUser>(sql: sqlQuery, parameters: parameters).ToList();

            foreach (var user in users)
            {
                user.RoleNames = GetEmployeeRoleNamesForUser(user.Id);
            }

            return View(users);
        }
      
        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            AppUser user = _db.Users.SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            user.RoleNames = GetEmployeeRoleNamesForUser(user.Id);
            
            AppValues.ImageViewModel imageView;

            if (user.ProfilePicture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(user.ProfilePicture)), null, null);
                user.ImagesBase64.Add(imageView);
            }


            return View(user);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            ViewBag.Roles = SetRolesDropDown();

            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel model)
        {
            int? roleId = model.Role;

            try
            {
                if (ModelState.IsValid)
                {
                    var user = new AppUser { UserName = model.Email, Email = model.Email, BirthDate = model.BirthDate.Value,
                        FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, AccountStatus = UserStatus.Enabled };

                    byte[] profilePicture = AppMethods.GetPicture(model.Files[0]);

                    // todo: Add saving photo into Photo tabel
                    if (profilePicture != null)
                    {
                        user.ProfilePicture = profilePicture;
                    }

                    var result = _userManager.Create(user, model.Password);
                    
                    if (result.Succeeded)
                    {
                        result = _userManager.AddToRole(user.Id, _roleManager.FindById(roleId.Value).Name);

                        if (result.Succeeded)
                        {
                            return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    AddErrors(result);
                }                
            }
            catch (InvalidDataException e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            // For modal always return error messages in JsonBadRequest object
            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }
        
        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            AppUser user = _userManager.FindById(id);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            user.RoleNames = GetEmployeeRoleNamesForUser(user.Id);
            int roleId = 0;

            if (user.RoleNames != null && user.RoleNames.Length > 0)
            {
                roleId = _roleManager.FindByName(user.RoleNames[0]).Id;
            }            
            
            EditViewModel viewModel = new EditViewModel { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, BirthDate = user.BirthDate,
                Email = user.Email, City = user.City, PostalCode = user.PostalCode, PhoneNumber = user.PhoneNumber, StreetName = user.StreetName, Role = roleId };

            AppValues.ImageViewModel imageView;

            if (user.ProfilePicture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(user.ProfilePicture)), null, null);
                viewModel.ImagesBase64.Add(imageView);
            }            

            ViewBag.Roles = SetRolesDropDown(roleId);
            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {

            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int? roleId = model.Role;

            // Get role assigned to model.
            string newRoleName = _roleManager.FindById<Role, int>(roleId.Value).Name;

            AppUser user = _userManager.FindById(model.Id);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.BirthDate = model.BirthDate.Value;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.City = model.City;
                    user.PostalCode = model.PostalCode;
                    user.StreetName = model.StreetName;

                    byte[] profilePicture = AppMethods.GetPicture(model.Files[0]);

                    if (user.ProfilePicture != null)
                    {
                        if (profilePicture != null && (!profilePicture.SequenceEqual(user.ProfilePicture)))
                        {
                            user.ProfilePicture = profilePicture;
                        }
                    }
                    else
                    {
                        user.ProfilePicture = profilePicture;
                    }

                    user.RoleNames = GetEmployeeRoleNamesForUser(user.Id);

                    var result = _userManager.Update(user);

                    if (result.Succeeded)
                    {
                        // If user is not in selected role, change it.
                        if (!_userManager.IsInRole(user.Id, newRoleName))
                        {
                            result = ChangeUserRole(user, newRoleName);

                            if (!result.Succeeded)
                            {
                                throw new InvalidOperationException();
                            }
                        }

                        // _db.SaveChanges();
                        return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                    }

                    AddErrors(result);
                }
            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError(String.Empty, String.Format("Error in assigning {0} role to {1} {2}", newRoleName, user.FirstName, user.LastName));
            }
            catch (InvalidDataException e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            // ViewBag.Roles = SetRolesDropDown(roleId);
           
            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }
   
        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            AppUser user = _db.Users.SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            user.RoleNames = GetEmployeeRoleNamesForUser(user.Id);

            AppValues.ImageViewModel imageView;

            if (user.ProfilePicture != null)
            {
                imageView = new AppValues.ImageViewModel(String.Format("data:image;base64,{0}", Convert.ToBase64String(user.ProfilePicture)), null, null);
                user.ImagesBase64.Add(imageView);
            }

            return View(user);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppUser user = _userManager.FindById(id);

            try
            {
                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                var result = _userManager.DisableAccountAsync(id);
                
                if (result.Succeeded)
                {
                    var creditCards = _db.CreditCards
                        .Where(c => c.UserId == id && c.Status != EntityStatus.Archived)
                        .ToList();

                    foreach(var card in creditCards)
                    {
                        card.Status = EntityStatus.Archived;
                        _db.Entry(card).State = EntityState.Modified;
                    }

                    _db.SaveChanges();

                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }

                AddErrors(result);
            }
            catch
            {
                ModelState.AddModelError(String.Empty, String.Format("Error in deleting {0} {1}", user.FirstName, user.LastName));
            }

            // For modal always return error messages in JsonBadRequest object
            var errors = AppMethods.GetModelStateErrorMsgs(ModelState);
            return new JsonBadRequest(new { errorMessages = errors });
        }

        #region Controller Helper Methods

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private int DeleteAllUserRoles(AppUser user)
        {
            user.RoleNames = GetUserRoleNames(user.Id);

            if (user.RoleNames != null && user.RoleNames.Length > 0)
            {
                var result = _userManager.RemoveFromRoles(user.Id, user.RoleNames);

                if (!result.Succeeded) throw new Exception();
            }

            return 1;
        }

        private bool IsAdmin(int userId)
        {
            return _userManager.IsInRole(userId, AppValues.AppRoles.ADMIN);
        }

        private bool IsEmployee(int userId)
        {
            return _userManager.IsInRole(userId, AppValues.AppRoles.EMPLOYEE);
        }

        private bool IsSuperEmployee(int userId)
        {
            return _userManager.IsInRole(userId, AppValues.AppRoles.SUPEREMPLOYEE);
        }
        private bool IsUser(int userId)
        {
            return _userManager.IsInRole(userId, AppValues.AppRoles.USER);
        }

        private string[] GetEmployeeRoleNamesForUser(int userId)
        {
            // Return roles that has employee keyword in them
            return _userManager.GetRoles(userId).Where(roleName => roleName.Contains(AppValues.AppRoles.EMPLOYEE)).ToArray<string>();
        }

        private SelectList SetRolesDropDown(object roleId = null)
        {
            List<Role> roles = _db.Roles
                .Where(r => r.Name != AppValues.AppRoles.ADMIN && r.Name != AppValues.AppRoles.USER)
                .OrderBy(r => r.Id)
                .ToList();

            if (roleId == null || (int)roleId < 1)
            {
                // If argument role id is null then by default select employee role
                return new SelectList(roles, "Id", "Name", _roleManager.FindByName(AppValues.AppRoles.EMPLOYEE).Id);
            }
            else
            {
                return new SelectList(roles, "Id", "Name", roleId);
            }
        }

        private string[] GetUserRoleNames(int userId)
        {
            return _userManager.GetRoles(userId).ToArray<string>();
        }

        private IdentityResult ChangeUserRole(AppUser user, string newRoleName)
        {
            IdentityResult result;

            if (user.RoleNames != null && user.RoleNames.Length > 0)
            {
                // Remove user from previous role
                result = _userManager.RemoveFromRole(user.Id, user.RoleNames[0]);

                if (!result.Succeeded) return result;
            }
                            
            result = _userManager.AddToRole(user.Id, newRoleName);            

            return result;
        }
        #endregion
    }
}

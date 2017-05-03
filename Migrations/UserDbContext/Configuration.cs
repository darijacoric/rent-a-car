namespace Migrations.UserDbContext
{
    using RentACar.App_Helpers;
    using RentACar.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RentACar.Models.UserDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\UserDbContext";
        }

        protected override void Seed(RentACar.Models.UserDbContext context)
        {

            var roles = new List<Role>()
            {
                new Role() { Name = "Admin" },
                new Role() { Name = "Employee" },
                new Role() { Name = "SuperEmployee" },
                new Role() { Name = "User" }
            };

            roles.ForEach(r => context.Roles.AddOrUpdate(x => x.Name, r));
            context.SaveChanges();

            var emptyDb = context.Users.Count() == 0;

            if (emptyDb == true && System.Web.HttpContext.Current.IsDebuggingEnabled)
            {
                var admin = new AppUser() { UserName = "admin@example.com", FirstName = "Admin", LastName = "Admin", BirthDate = DateTime.Parse("1.1.1980"), Email = "admin@example.com", AccountStatus = UserStatus.Enabled };

                string passwd = "Pa$$w0rd_1";

                RentACar.Controllers.AccountController controller = new RentACar.Controllers.AccountController();
                controller.UserManager.CreateAsync(admin, passwd);
            }
          
            var paymentTypes = new List<PaymentType>()
            {
                //new PaymentType() { Name = AppValues.PaymentTypes.CASH, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New },
                new PaymentType() { Name = AppValues.PaymentTypes.PAYPAL, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New },
                new PaymentType() { Name = AppValues.PaymentTypes.VISA, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New },
                new PaymentType() { Name = AppValues.PaymentTypes.ERSTE, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New },
                new PaymentType() { Name = AppValues.PaymentTypes.PBZ, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New },
                new PaymentType() { Name = AppValues.PaymentTypes.AMERICAN, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New },
                new PaymentType() { Name = AppValues.PaymentTypes.MAESTRO, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New },
                new PaymentType() { Name = AppValues.PaymentTypes.MASTERCARD, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = EntityStatus.New }
            };

            paymentTypes.ForEach(p => context.PaymentTypes.AddOrUpdate(x => x.Name, p));
            context.SaveChanges();
        }
    }
}

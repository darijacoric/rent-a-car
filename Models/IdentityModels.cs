using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using RentACar.App_Helpers;

namespace RentACar.Models
{
    public enum UserStatus { Enabled = 1, Disabled = 0 }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AppUser : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        public AppUser()
        {
            ImagesBase64 = new List<AppValues.ImageViewModel>();
        }

        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime BirthDate { get; set; }
                                                
        [ScaffoldColumn(false)]
        [Display(Name = "Profile Picture")]
        [DataType(DataType.ImageUrl)]
        public byte[] ProfilePicture { get; set; }

        [StringLength(16)]
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [StringLength(30)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(40)]
        [Display(Name = "Street Name")]
        public string StreetName { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(30)]
        [Display(Name = "State")]
        public string State { get; set; }

        //[RegularExpression(@"^[0-9]*", ErrorMessage = "Input only numbers for Phone Number.")]        
        [Phone()]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(40)]
        public override string PhoneNumber
        {
            get
            {
                return base.PhoneNumber;
            }

            set
            {
                base.PhoneNumber = value;
            }
        }

        public UserStatus? AccountStatus { get; set; }
        
        public virtual ICollection<CreditCard> CreditCards { get; set; }

        [Display(Name = "Address")]
        public string Address
        {
            get
            {
                return String.Format("{0} {1} {2}", !String.IsNullOrEmpty(StreetName) ? StreetName + "," : "", !String.IsNullOrEmpty(City) ? City + "," : "", !String.IsNullOrEmpty(State) ? State + "," : "");
            }
        }

        [Display(Name = "Full Name")]
        public string FullName { get { return String.Format("{0} {1}", FirstName, LastName); } }
                
        [NotMapped()]
        [ScaffoldColumn(false)]
        public string[] RoleNames { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public List<AppValues.ImageViewModel> ImagesBase64 { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public List<Photos> Photos { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


    }

    public class UserRole : IdentityUserRole<int> { }
    public class UserLogin : IdentityUserLogin<int> { }
    public class UserClaim : IdentityUserClaim<int> { }
    public class Role : IdentityRole<int, UserRole>
    {
        public Role() { }
        public Role(string name) { Name = name; }
    }

    public class CustomUserStore : UserStore<AppUser, Role, int, UserLogin, UserRole, UserClaim>
    {
        public CustomUserStore(UserDbContext context)
            : base (context)
        {
        }        
    }

    public class CustomRoleStore : RoleStore<Role, int, UserRole>
    {
        public CustomRoleStore(UserDbContext context)
            : base (context)
        {
        }
    }
    
    public class CustomUserManager : UserManager<AppUser, int>
    {
        public CustomUserManager(CustomUserStore store)
            : base (store)
        {

        }
                
        public virtual IdentityResult DisableAccountAsync(int userId)
        {
            // todo: Add messages from DB
            List<string> errorMessages = new List<string>();            
                        
            var user = this.FindById(userId);

            if (user == null)
            {
                errorMessages.Add("User doesn't exist in database.");
            }
            else
            {
                user.AccountStatus = UserStatus.Disabled;                
            }

            return errorMessages.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errorMessages.ToArray());
            
        }
    }

    public class CustomRoleManager : RoleManager<Role, int>
    {
        public CustomRoleManager(CustomRoleStore store) 
            : base (store)
        {

        }
    }
    
    public class UserDbContext : IdentityDbContext<AppUser, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserDbContext()
            : base("UsersConnection")
        {
        }

        public static UserDbContext Create()
        {
            return new UserDbContext();
        }
                
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<AppUser>().ToTable("Users").HasKey(x => x.Id);
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<Role>().ToTable("Roles");

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<CreditCard>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("CreditCards");

            });

        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // if entry is derived from EntityBaseClass. Some models are not derived like AppParamValues
                // thus they don't have CreatedDate, ModifiedDate columns.
                if (entry.Entity is EntityBaseClass)
                {
                    EntityBaseClass entity = (EntityBaseClass)entry.Entity;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedDate = DateTime.Now;
                        entity.ModifiedDate = DateTime.Now;
                        entity.Status = EntityStatus.New;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        if (entity.SetDates == true)
                        {
                            entity.ModifiedDate = DateTime.Now;
                            // entity.Status = EntityStatus.Active;
                        }
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        entity.ModifiedDate = DateTime.Now;
                        entity.Status = EntityStatus.Archived;
                    }
                }
                // todo: add logging for each operation. See http://www.entityframeworktutorial.net/change-tracking-in-entity-framework.aspx for code example

            }

            return base.SaveChanges();
        }

        private void SetEntityDates<T, U>(T entity, U entry) where T : EntityBaseClass where U : System.Data.Entity.Infrastructure.DbEntityEntry
        {
            entity = (T)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = EntityStatus.Active;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.ModifiedDate = DateTime.Now;
                entity.Status = EntityStatus.Active;
            }
        }
    }        
}
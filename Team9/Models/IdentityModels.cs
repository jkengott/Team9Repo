﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

//TODOXX: Change the namespace here to match your project's name
namespace Team9.Models
{
    // You can add profile data for the user by adding more properties to your AppUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AppUser : IdentityUser
    {
        //TODO: Put any additional fields that you need for your users here
        //For example:
        public string FName { get; set; }
        public string LName { get; set; }
        public virtual CreditCard CC1 { get; set; }
        public virtual CreditCard CC2 { get; set; }
        public string SSN { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StreeAddress { get; set; }
        public string Zip { get; set; }

        public virtual List<Purchase> Purchases { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    //NOTE: Here is your dbContext for the entire project.  There should only be ONE DbContext per project
    //Your dbContext (AppDbContext) inherits from IdentityDbContext, which inherits from the "regular" DbContext
    //TODO: If you have an existing dbContext (it may be in your DAL folder), DELETE THE EXISTING dbContext

    public class AppDbContext : IdentityDbContext<AppUser>
    {
        //TODO: Add your dbSets here.  As an example, I've included one for products
        //Remember - the IdentityDbContext already contains a db set for Users.  If you add another one, your code will break
        //public DbSet<Product> Products { get; set; }
                
        public AppDbContext()
            : base("MyDbConnection", throwIfV1Schema: false)
        {
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
        
        //Add dbSet for roles
         public DbSet<AppRole> AppRoles { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.Artist> Artists { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.Album> Albums { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.Song> Songs { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.Purchase> Purchases { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.PurchaseItem> PurchaseItems { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.Genre> Genres { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.Rating> Ratings { get; set; }

        public System.Data.Entity.DbSet<Team9.Models.CreditCard> Creditcards { get; set; }


    }
}

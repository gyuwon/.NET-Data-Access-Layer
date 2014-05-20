namespace Commerce.Migrations
{
    using System.Data.Entity.Migrations;
    using Commerce.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context);
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore);

            string administrator = "Administrator";
            IdentityRole role = roleManager.FindByName(administrator);
            if (role == null)
                roleManager.Create(new IdentityRole(administrator));

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            string userName = "admin";
            ApplicationUser user = userManager.FindByName(userName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = userName };
                userManager.Create(user, password: "123$%^qwe");
            }

            if (userManager.IsInRole(user.Id, administrator) == false)
                userManager.AddToRole(user.Id, administrator);
        }
    }
}

namespace PocketGraphs.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using PocketGraphs.Models;
    using PocketGraphs.Models.Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<PocketGraphs.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "PocketGraphs.Models.ApplicationDbContext";
        }

        protected override void Seed(PocketGraphs.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.


            //Commented out code is left here testing purposes

            //var post = new Post();
            //post.Title = "2";
            //post.Body = "Drama";
            //post.Published = false;
            //post.MediaUrl = "haha";


            //context.Posts.AddOrUpdate(p => p.Title, post);


            //Seeding Users and Roles

            //RoleManager, used to manage roles
            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            //UserManager, used to manage users
            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));

            //Adding admin role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Admin" && p.Name == "Moderator"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
                var moderatorRole = new IdentityRole("Moderator");
                roleManager.Create(moderatorRole);
            }

            //Creating the adminuser
            ApplicationUser adminUser;
            ApplicationUser moderatorUser;


            if (!context.Users.Any(
                p => p.UserName == "admin@pocketgraphs.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@pocketgraphs.com";
                adminUser.Email = "admin@pocketgraphs.com";
                userManager.Create(adminUser, adminUser.Email);
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@pocketgraphs.com");
            }


            if (!context.Users.Any(
                p => p.UserName == "moderator@pocketgraphs.com"))
            {
                moderatorUser = new ApplicationUser();
                moderatorUser.UserName = "moderator@pocketgraphs.com";
                moderatorUser.Email = "moderator@pocketgraphs.com";
                userManager.Create(moderatorUser, moderatorUser.Email);
            }
            else
            {
                moderatorUser = context
                    .Users
                    .First(p => p.UserName == "moderator@pocketgraphs.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            if (!userManager.IsInRole(moderatorUser.Id, "Moderator"))
            {
                userManager.AddToRole(moderatorUser.Id, "Moderator");
            }


        }
    }
}

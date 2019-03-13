namespace BlogAssignment.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using BlogAssignment.Models;
    using BlogAssignment.Models.Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<BlogAssignment.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "BlogAssignment.Models.ApplicationDbContext";
        }

        protected override void Seed(BlogAssignment.Models.ApplicationDbContext context)
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
            if (!context.Roles.Any(p => p.Name == "Admin"  && p.Name == "Moderator"))
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
                p => p.UserName == "guilherme.guizado@mitt.ca"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "guilherme.guizado@mitt.ca";
                adminUser.Email = "guilherme.guizado@mitt.ca";
                userManager.Create(adminUser, "Password-1");                
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "guilherme.guizado@mitt.ca");                
            }


            if (!context.Users.Any(
                p => p.UserName == "moderator@blog.com"))
            {
                moderatorUser = new ApplicationUser();
                moderatorUser.UserName = "moderator@blog.com";
                moderatorUser.Email = "moderator@blog.com";
                userManager.Create(moderatorUser, "Password-1");
            }
            else
            {
                moderatorUser = context
                    .Users
                    .First(p => p.UserName == "moderator@blog.com");
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

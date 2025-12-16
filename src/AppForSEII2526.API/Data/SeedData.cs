using AppForSEII2526.API.Models;
using AppForSEII2526.Models;
using Humanizer.Localisation;

namespace AppForSEII2526.API.Data
{
    public static class SeedData
    {

        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger)
        {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try
            {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try
            {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }


        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles)
        {

            foreach (string roleName in roles)
            {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles)
        {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null)
            {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es", "Avda. España 2, Albacete");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }

            if (userManager.FindByNameAsync("gregorio@uclm.es").Result == null)
            {
                ApplicationUser user = new ApplicationUser("2", "Gregorio", "Diaz Descalzo", "gregorio@uclm.es", "Avda. España 25, Ciudad Real");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "APassword1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    //employee role
                    userManager.AddToRoleAsync(user, roles[1]).Wait();
                }
            }

            if (userManager.FindByNameAsync("peter@uclm.es").Result == null)
            {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es", "Avda. España 75, London");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

        }

        /*
        public static void SeedModelsAndCars(ApplicationDbContext dbcontext)
        {
            string[] modelnames = ["Model S", "R8", "Mustang"];
            List<Model> models = [];
            Car car;
            foreach (string modelname in modelnames)
            {
                var model = dbcontext.Model.FirstOrDefault(m => m.Name == modelname);
                if (model == null)
                    models.Add(new Model(modelname));
                else
                    models.Add(model);
            }
            if (dbcontext.Cars.FirstOrDefault(c => c.RentingPrice == 45000) == null)
            {
                car = new Car("The lord of the rings", models[0], new DateTime(2011, 10, 20), 10.0m, 5, 1.0m, 2);
                car = new Car("The last of us", models[0], new DateTime(2023, 3, 15), 10.0m, 5, 1.0m, 2);
                dbcontext.Cars.Add(car);

            }

            if (dbcontext.Cars.FirstOrDefault(c => c.RentingPrice == 35000) == null)
            {
                car = new Car("The mechanic orange", models[1], new DateTime(1988, 02, 23), 15.0m, 10, 2.0m, 10);
                car = new Car("The man in the high castle", models[1], new DateTime(2015, 01, 15), 15.0m, 10, 3.0m, 10);
                dbcontext.Cars.Add(car);
            }

            //it saves the modification of dbcontext to the database
            dbcontext.SaveChanges();

            //alternatively you may have used a raw SQL
            //dbcontext.Database.ExecuteSqlRaw("INSERT INTO [Movies] ([Id], [Title], [GenreId], [ReleaseDate], [PriceForPurchase], [QuantityForPurchase], [PriceForRenting], [QuantityForRenting]) VALUES (1, N'The lord of the rings', 1, N'2011-10-20 00:00:00', 10, 1000, 1, 100)");
            //dbcontext.Database.ExecuteSqlRaw("INSERT INTO [Movies] ([Id], [Title], [GenreId], [ReleaseDate], [PriceForPurchase], [QuantityForPurchase], [PriceForRenting], [QuantityForRenting]) VALUES (2, N'The flying castle', 2, N'2007-04-04 00:00:00', 20, 1000, 3, 10)");


            //Since EFCORE7, you can perform bulk updates with linq.
            dbcontext.Cars.ExecuteUpdate(s => s.SetProperty(m => m.QuantityForPurchasing, 10));

            //other example using existing information: add 100 to the QuantityForPurchase of each Movie
            //dbcontext.Movies.ExecuteUpdate(s => s.SetProperty(m => m.QuantityForPurchase, m=>m.QuantityForPurchase+100));

            //You can alternatively use raw SQL to perform the operation where performance is sensitive:
            //dbcontext.Database.ExecuteSqlRaw("UPDATE [Movies] SET [QuantityForPurchase] = 100");

            dbcontext.SaveChanges();


        }
        */
    }
}
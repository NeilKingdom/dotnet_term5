using Assignment2.Data;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*** Add services to the container ***/

            // Add support for MVC
            builder.Services.AddControllersWithViews(options => { 
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // Validate Anti Forgery on all form pages
            });

            // Add services required for session state
            builder.Services.AddSession();

            // Setup DB connection
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<MarketDbContext>(options => options.UseSqlServer(connection));

            // Setup Azure blob storage connection
            var blobConnection = builder.Configuration.GetConnectionString("AzureBlobStorage");
            builder.Services.AddSingleton(new BlobServiceClient(blobConnection));
            
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<MarketDbContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured while seeding the database.");
                }
            }

            /*** Configure the HTTP request pipeline ***/
            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/Home/Error");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });

            app.Run();
        }
    }
}
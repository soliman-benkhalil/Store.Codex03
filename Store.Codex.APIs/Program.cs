
using Microsoft.EntityFrameworkCore;
using Store.Codex.Core;
using Store.Codex.Core.Mapping;
using Store.Codex.Core.Services.Contract;
using Store.Codex.Repository;
using Store.Codex.Repository.Data;
using Store.Codex.Repository.Data.Contexts;
using Store.Codex.Service.Services.Products;

namespace Store.Codex.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<StoreDbContext>(optoin =>
            {
                optoin.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddAutoMapper(M => M.AddProfile(new ProductProfile(builder.Configuration)));

            var app = builder.Build();

            //StoreDbContext context = new StoreDbContext(); // there is an error here because i have to make an object from StoreDbContext so i will ask him in a different way
            //context.Database.MigrateAsync(); // this line apply any migrations that is not applied yet and if the db does not exist it creates it (Update Databaes By the code)

            using var scope = app.Services.CreateScope(); // here i make a container from the services that work by scope lifetime
                                                          // and it is unmanaged resource to i used using (mean i need to close it after operation
                                                          // and it does not close by default) -> AddDbContext<StoreDbContext> by default is scope

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<StoreDbContext>();
            //await context.Database.MigrateAsync(); // it applies the migrations in DB and create DB if it is not existed .... but what if it is existed so try and catch

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    
            try
            {
                await context.Database.MigrateAsync();
                await StoreDbContextSeed.SeedAsync(context);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>(); // we can make a message by print but it will appear on the command but use logger is better for make the message more expressed
                // and determine the name of the class that i make the logger in it and it is here called program not the Main because Main is a function not a class because the class is the responisble to make the console screen

                logger.LogError(ex, "There are problems during apply migrations !");


            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

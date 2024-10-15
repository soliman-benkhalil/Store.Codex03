using Microsoft.EntityFrameworkCore;
using Store.Codex.Core.Services.Contract;
using Store.Codex.Core;
using Store.Codex.Repository;
using Store.Codex.Repository.Data.Contexts;
using Store.Codex.Service.Services.Products;
using Store.Codex.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Store.Codex.APIs.Errors;
using Store.Codex.Core.Repositories.Contract;
using StackExchange.Redis;
using Store.Codex.Core.Mapping.Basket;
using Store.Codex.Repository.Repositories;

namespace Store.Codex.APIs.Helper
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddDependency(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddBuiltInService();
            services.AddSwaggerService();
            services.AddDbContextService(configuration);
            services.AddUserDefinedService();
            services.AddAutoMapperService(configuration);
            services.ConfigureInvalidModelStateResponseService();
            services.AddRedisService(configuration);

            return services;

        }

        private static IServiceCollection AddBuiltInService(this IServiceCollection  services)
        {
            services.AddControllers();
            return services;
        }

        private static IServiceCollection AddSwaggerService(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer(); // it is to let the swagger to c all endpoints in the all controllers   
            services.AddSwaggerGen();
            return services;
        }

        private static IServiceCollection AddDbContextService(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(optoin =>
            {
                optoin.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }

        private static IServiceCollection AddUserDefinedService(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBasketRepository,BasketRepository>();  
            return services;
        }

        private static IServiceCollection AddAutoMapperService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(M => M.AddProfile(new ProductProfile(configuration)));
            services.AddAutoMapper(M => M.AddProfile(new BasketProfile()));

            return services;
        }

        private static IServiceCollection ConfigureInvalidModelStateResponseService(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>  // ApiBehaviorOptions -> it represents the behavior response of api
            {
                options.InvalidModelStateResponseFactory = (ActionContext) => // InvalidModelStateResponseFactory -> it is the object that is responsible of the shape of any error related with the model state
                {
                    var errors = ActionContext.ModelState.Where(P => P.Value.Errors.Count() > 0) // condtion where there are errors or not
                                            .SelectMany(P => P.Value.Errors)
                                            .Select(E => E.ErrorMessage)
                                            .ToArray();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response); // it is the solution here for the error got by this line BadRequest(response)
                    // BadRequest(response) -> we have problem with the type of badrequest we cant use badrequest as a method
                    // and that is why we used it before because we used it in a controller that inherit from the controller base and it is not happened here 
                };
            });
            return services;
        }

        private static IServiceCollection AddRedisService(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>
            (
                (serviceProvider) =>
                {
                    var connect = configuration.GetConnectionString("Redis");
                    
                    return ConnectionMultiplexer.Connect(connect);
                }
            ); 
            return services;
        }

    }
}

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Domain.Internals.Repository;
using Warehouse.Domain.Internals.Repository.DataAccess;
using Warehouse.Domain.Internals.Repository.Handlers;
using Warehouse.Domain.Internals.Repository.Handlers.Abstractions;
using Warehouse.Domain.Internals.Service;
using Warehouse.Domain.Internals.Service.Handlers;
using Warehouse.Domain.Internals.Service.Handlers.Abstractions;

[assembly: InternalsVisibleTo("Warehouse.Domain.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Warehouse.Domain
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddWarehouseDomain(this IServiceCollection services)
        {
            services.AddDbContext<WarehouseDbContext>();

            services.AddTransient<IUpdateArticleQuantityHandler, UpdateArticleQuantityHandler>();
            services.AddTransient<IGetProductHandler, GetProductHandler>();
            services.AddTransient<IGetProductsHandler, GetProductsHandler>();
            services.AddTransient<IAddUpdateProductsHandler, AddUpdateProductsHandler>();
            services.AddTransient<IAddUpdateArticlesHandler, AddUpdateArticlesHandler>();
            services.AddTransient<IGetArticlesHandler, GetArticlesHandler>();

            services.AddTransient<IWarehouseRepository, WarehouseRepository>();

            services.AddTransient<IImportProductsHandler, ImportProductsHandler>();
            services.AddTransient<IGetProductStocksHandler, GetProductStocksHandler>();
            services.AddTransient<IOrderProductHandler, OrderProductHandler>();
            services.AddTransient<IProductsService, ProductsService>();

            services.AddTransient<IImportInventoryHandler, ImportInventoryHandler>();
            services.AddTransient<IInventoryService, InventoryService>();

            return services;
        }

        public static IApplicationBuilder AddWarehouseDomain(this IApplicationBuilder app)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<WarehouseDbContext>();
            dbContext.Database.EnsureCreated();
            return app;
        }
    }
}

using Store.Codex.Core.Entities;
using Store.Codex.Repository.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.Codex.Repository.Data
{
    public class StoreDbContextSeed
    {
        // It is important to start read the data that has no dependancies 
        public async static Task SeedAsync(StoreDbContext _context)
        {

            if (_context.Brands.Count() == 0)
            {
                // Brand
                // 1. Read Data From Json File

                var brandData = File.ReadAllText(@"..\Store.Codex.Repository\Data\DataSeed\brands.json");

                // 2. Convert Json string To List<T>

                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);

                // 3. Seed Data To DB

                if (brands is not null && brands.Count() > 0)
                {
                    await _context.Brands.AddRangeAsync(brands);// AddRangeAsync Takes A Sequence And Add It To The DB by change the state of each object from deattached to added
                }
            }

            if (_context.Types.Count() == 0)
            {
                // Brand
                // 1. Read Data From Json File

                var typeData = File.ReadAllText(@"..\Store.Codex.Repository\Data\DataSeed\types.json");

                // 2. Convert Json string To List<T>

                var types = JsonSerializer.Deserialize<List<ProductType>>(typeData);

                // 3. Seed Data To DB

                if (types is not null && types.Count() > 0)
                {
                    await _context.Types.AddRangeAsync(types);
                }
            }

            if (_context.Products.Count() == 0)
            {
                // Brand
                // 1. Read Data From Json File

                var productData = File.ReadAllText(@"..\Store.Codex.Repository\Data\DataSeed\products.json");

                // 2. Convert Json string To List<T>

                var products = JsonSerializer.Deserialize<List<Product>>(productData);

                // 3. Seed Data To DB

                if (products is not null && products.Count() > 0)
                {
                    await _context.Products.AddRangeAsync(products);
                }
            }

            await _context.SaveChangesAsync();

        }
    }
}

using Core.DataAccess.EntityFramework.Repository;
using DataAccess.EntityFramework;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class ProductRepository:Repository<Product, ET_StoreARP>, IProductDal
    {
        //public ProductRepository(ET_StoreARP context) : base(context)
        //{
        //}
      
        public async Task<List<Product>> GetStockAndProduct()
        {
                string year = DateTime.Now.Year.ToString();
            using (ET_StoreARP context = new ET_StoreARP())
            {

                var productAndStock = await(from a in context.Products
                        join b in context.Stocks on a.Id equals b.ProductId
                        join c in context.Colors on b.ColorId equals c.Id
                        where a.Id==b.ProductId&& b.ColorId==c.Id &&b.TekliMi==null&& b.MagazaMi==null&&b.StockYear==year
                        select new Product()
                        {
                            Id=a.Id,
                            IsActive=a.IsActive,
                             Age=a.Age,
                             ModelCode=a.ModelCode,
                             ModelColor=c.ColorName,
                             ModelCount=a.ModelCount,
                             Gender=a.Gender,
                             ModelName=a.ModelName,
                             RegisterDate=a.RegisterDate,
                             Stock=new Stock
                             {
                                 ProductId=a.Id,
                                 RegisterDate=b.RegisterDate,
                                 //TenantId=b.TenantId,
                                 StockCount=b.StockCount
                             }
                             
                        }).AsNoTracking().OrderByDescending(a => a.Age).ToListAsync();
        
                return productAndStock;
            }
        }



        public async Task<List<Product>> GetProductStockUpdate(long id)
        {
            string year = DateTime.Now.Year.ToString();
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var productAndStock = await (from a in context.Products.Include(a=>a.ProductAges)
                                             join b in context.Stocks on a.Id equals id
                                             join c in context.Colors on b.ColorId equals c.Id
                                             where a.Id == id && b.ProductId == id&& b.TekliMi==null && b.MagazaMi == null&&b.StockYear==year
                                             select new Product()
                                             {
                                                 Id = id,
                                                 IsActive = a.IsActive,
                                                 ProductAges = a.ProductAges,
                                                 ModelCode = a.ModelCode,
                                                 ModelColor = c.ColorName,
                                                 ModelCount = b.StockCount.ToString(),
                                                 Gender = a.Gender,
                                                 ModelName = a.ModelName,
                                                 RegisterDate = a.RegisterDate,
                                                 Stock =  new Stock
                                                 {
                                                      ProductId = a.Id,
                                                     RegisterDate = b.RegisterDate,
                                                     //TenantId = b.TenantId,
                                                     StockCount = b.StockCount
                                                 },
                                                 Colors = new List<Color>
                                                 {
                                                     new Color
                                                     {
                                                     ColorName=c.ColorName,
                                                     Id=c.Id
                                                     }
                                                 }
                                                

                                             }).AsNoTracking().ToListAsync();

                return productAndStock;
            }
        }


        public async Task<List<Product>> GetProductStock(long id)
        {
            string year = DateTime.Now.Year.ToString();
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var productAndStock = await (from a in context.Products.Include(a=>a.ProductAges)
                                             join b in context.Stocks on a.Id equals id
                                             join c in context.Colors on b.ColorId equals c.Id
                                             join x in context.ProductAges on a.ProductAges.Id equals x.Id
                                             where a.Id==id && b.ProductId ==id && b.TekliMi == null && b.MagazaMi == null&&b.StockYear==year
                                             select new Product()
                                             {
                                                 Id = id,
                                                 IsActive = a.IsActive,
                                                 Age = a.Age,
                                                 Barcode = a.Barcode,
                                                 ModelImageUrl = a.ModelImageUrl,
                                                 ModelCode = a.ModelCode,
                                                 ModelColor = c.ColorName,
                                                 ModelCount = b.StockCount.ToString(),
                                                 Gender = a.Gender,
                                                 ProductAges=a.ProductAges,
                                                 ModelName = a.ModelName,
                                                 RegisterDate = a.RegisterDate,
                                                 Stock = new Stock
                                                 {
                                                     ProductId = a.Id,
                                                     RegisterDate = b.RegisterDate,
                                                     //TenantId = b.TenantId,
                                                     StockCount = b.StockCount,
                                                     RenkBarcode=b.RenkBarcode
                                                 },
                                                 Colors = new List<Color>
                                                 {
                                                     new Color
                                                     {
                                                     ColorName=c.ColorName,
                                                     Id=c.Id
                                                     }
                                                 }


                                             }).AsNoTracking().OrderByDescending(a=>a.Age).ToListAsync();

                return productAndStock;
            }
        }

        public async Task<List<Product>> OnlyMagaza(long id)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var productAndStock = await (from a in context.Products
                                             join b in context.MagazaStocks on a.Id equals id
                                             join c in context.Colors on b.ColorId equals c.Id
                                             where a.Id == id && b.ProductId == id
                                             select new Product()
                                             {
                                                 Id = id,
                                                 IsActive = a.IsActive,
                                                 Age = a.Age,
                                                 ModelCode = a.ModelCode,
                                                 Barcode = a.Barcode,
                                                 ModelColor = c.ColorName,
                                                 ModelCount = b.StockCount.ToString(),
                                                 Gender = a.Gender,
                                                 ModelName = a.ModelName,
                                                 RegisterDate = a.RegisterDate,
                                                 ModelImageUrl=a.ModelImageUrl,
                                                 Stock = new Stock
                                                 {
                                                     ProductId = a.Id,
                                                     RegisterDate = b.RegisterDate,
                                                     //TenantId = b.TenantId,
                                                     StockCount = b.StockCount
                                                 },
                                                 Colors = new List<Color>
                                                 {
                                                     new Color
                                                     {
                                                     ColorName=c.ColorName,
                                                     Id=c.Id
                                                     }
                                                 }


                                             }).AsNoTracking().ToListAsync();

                return productAndStock;
            }
        }
        
        public async Task<ShippingProduct> GetTekliDetay(long tekliId)
        {
            string year = DateTime.Now.Year.ToString();
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var result = await(from s in context.Stocks
                                    join p in context.Products on s.ProductId equals p.Id
                                    join c in context.Colors on s.ColorId equals c.Id
                                    where s.ColorId==c.Id && s.ProductId==p.Id && s.tekliId==tekliId/*&&s.StockYear==year*/
                                        select new ShippingProduct
                                        {
                                            ProductId=p.Id,
                                            ColorId=c.Id,
                                            TekliId=s.Id,
                                            Count=s.StockCount,
                                           ProductName=p.ModelName,
                                           ColorName=c.ColorName
                                        }).FirstOrDefaultAsync();
                return result;
            }
    

        }

        public async Task<ShippingProduct> GetTekliAll(long tekliId)
        {
            string year = DateTime.Now.Year.ToString();
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var result = await (from s in context.Stocks
                                    join p in context.Products.Include(a=>a.ProductAges) on s.ProductId equals p.Id
                                    join c in context.Colors on s.ColorId equals c.Id
                                    //join age in context.ProductAges on p.ProductAges.Id equals c
                                    where s.ColorId == c.Id && s.ProductId == p.Id && s.tekliId == tekliId /*&& s.StockYear == year*/
                                    select new ShippingProduct
                                    {
                                        ProductId = p.Id,
                                        ColorId = c.Id,
                                        TekliId = s.Id,
                                        Count = s.StockCount,
                                        ProductName = p.ModelName,
                                        ColorName = c.ColorName,
                                        Age = p.ProductAges.Name,
                                         Gender= p.Gender,
                                         Barcode= p.Barcode,
                                         ModelCode= p.ModelCode
                                    }).FirstOrDefaultAsync();
                return result;
            }


        }

        public async Task<Product> GetProductIdByAge(long productId)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var result = await context.Products.Include(a => a.ProductAges).FirstOrDefaultAsync(s => s.Id == productId);
                return result;
            }
        }
    }
}

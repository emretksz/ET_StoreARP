using Application.Interface;
using AutoMapper;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Castle.Components.DictionaryAdapter.Xml;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using static Entities.Enums.Enums;
using PagedList;

namespace Application.Services
{
    public class ProductManager : IProductServices
    {
        private readonly IProductDal _productRepository;
        private readonly StockManager _stockManager=new StockManager(new StockRepository());
        private readonly ColorManager _colorManager=new ColorManager(new ColorRepository());

        private readonly IMapper _mapper;

        public ProductManager(IProductDal productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
       
        }

        [ValidationAspect(typeof (ProductValidator))]
        public async Task<long> Add(Product product)
        {
            try
            {
                if (product != null)
                {
                    
                    //var user = await _userManager.FindByNameAsync(product.UserName);
           
                    var ids = await _productRepository.CreateAsyncReturnId(product);
                           // await FirstStock(ids,Convert.ToInt64(product.ModelCount));
                    //TenantId login olan user'dan bulacağım. IdentityUser manupule edilmeli
                    //Stock stock = new()
                    //        {
                    //            ProductId = ids,
                    //            RegisterDate = DateTime.Now.ToString(),
                    //            TenantId = 1,

                    //        };

                    //await _stockManager.Add(stock);
                    return ids;
                }
                return 0;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        public async Task<bool> FirstStock(long Id, long count)
        {
            try
            {
                var product = await _productRepository.GetByFilterAsync(a => a.Id == Id);
                if (product != null)
                {
                    var totalStock = count;
                    Stock stocks = new()
                    {
                        ProductId = Id,
                        RegisterDate = DateTime.Now.ToString(),
                        //TenantId = 1,
                        StockCount = totalStock
                    };
                    await _stockManager.Add(stocks);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

            public async Task<bool> AddStockCount(Stock stock)
        {
            try
            {
                var product = await _productRepository.GetByFilterAsync(a=>a.Id==stock.ProductId);
                if (product!=null)
                {
                    var totalStock = Convert.ToInt64(product.ModelCount!=""?product.ModelCount:"0") + stock.StockCount;
                    Product product1 = new Product
                    {
                        ModelCount = stock.ToString()
                    };
                    //product.ModelCount = stock.ToString();
                    _productRepository.Update(product1, product);
                    Stock stocks = new()
                    {
                        ProductId = stock.ProductId,
                        RegisterDate = DateTime.Now.ToString(),
                        //TenantId = 1,
                        StockCount=totalStock

                    };
                   await _stockManager.Add(stocks);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IResult> Delete(Product product)
        {
            try
            {
                if (product != null)
                {
                    //colors,ship,stock
                    var stock = await _stockManager.GetAll(a => a.ProductId == product.Id);
                    foreach (var item in stock.Data)
                    {
                        await _stockManager.Delete(item);
                    }
                    ShippingManager _shippingManager = new ShippingManager(_productRepository, _mapper, new ShippingRepository());


                    //var shipping = await _shippingManager.GetAll(a => a.ProductId == product.Id);
                    //foreach (var item in shipping.Data)
                    //{
                    //    await _shippingManager.Delete(item);
                    //}

                    _productRepository.Remove(product);
                    return new SuccessResult();
                }
                return new ErrorResult(ConstMessages.ProductRemoveError);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public  async Task<IDataResults<List<Product>>> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            try
            {
                if (filter != null)
                {
                    return new SuccessDataResult<List<Product>>(await _productRepository.GetAllAsync(filter), "Ürünler Listelendi");
                }
                return new SuccessDataResult<List<Product>>(await _productRepository.GetAllAsync(), "Ürünler Listelendi");
            }
            catch (Exception)
            {

                return null;
            }

        }

        //public async Task<IDataResults<List<Product>>> GetByCategorId(int id)
        //{
        //    if (id!=0)
        //    {
        //        return new SuccessDataResult<List<Product>>( await _productRepository.GetByFilterAsync(a => a.Id == id));
        //    }
        //}

        public async Task<IDataResults<Product>> GetById(long productId)
        {
            try
            {
                if (productId != 0)
                {
                    return new SuccessDataResult<Product>(await _productRepository.GetByFilterAsync(a => a.Id == productId));
                }
                return null;
            }
            catch (Exception)
            {

                return null ;
            }
        }

        public async Task<List<Product>> GetStockAndProduct()
        {
            return await _productRepository.GetStockAndProduct();
        }

        [ValidationAspect(typeof(ProductValidator))]
        public async Task<IResult> Update(Product product)
        {
            try
            {
                if (product != null)
                {
                    var result = await _productRepository.GetByFilterAsync(a => a.Id == product.Id);
                    product.RegisterDate = DateTime.Now.ToString();
                    _productRepository.Update( product, result);
                    //var user = await _userManager.FindByNameAsync(product.UserName);
                    //TenantId login olan user'dan bulacağım. IdentityUser manupule edilmeli
                    //Stock stock = new()
                    //{
                    //    RegisterDate = DateTime.Now.ToString(),
                    //    TenantId = 1,
                    //    ProductId = product.Id,  
                    //};

                    //await _stockManager.Update(stock);
                    return new  SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<List<Product>> GetProductStockUpdate(long id)
        {
            return await _productRepository.GetProductStockUpdate(id);
        }

        public async Task<IResult> UpdateAll(Product product)
        {
            try
            {
                if (product != null)
                {
                    product.RegisterDate = DateTime.Now.ToString();
                    //var result = await _productRepository.GetByFilterAsync(a => a.Id == product.Id);
                    _productRepository.UpdateAll(product);
                    //var user = await _userManager.FindByNameAsync(product.UserName);
                    //TenantId login olan user'dan bulacağım. IdentityUser manupule edilmeli
                    //Stock stock = new()
                    //{
                    //    RegisterDate = DateTime.Now.ToString(),
                    //    TenantId = 1,
                    //    ProductId = product.Id,
                    //};

                    //await _stockManager.Update(stock);
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<List<Product>> GetProductStock(long id)
        {
            return await _productRepository.GetProductStock(id);
        }

        public async Task<List<Product>> OnlyMagaza(long id)
        {
            return await _productRepository.OnlyMagaza(id);
        }

        public async Task<ShippingProduct> GetTekliDetay(long tekliId)
        {
            return await _productRepository.GetTekliDetay(tekliId);
        }

        public async Task<IQueryable<Product>> GetFilterProduct(SearchParticalDto p,int page,int size)
        {
       
            var query = _productRepository.GetQuery();

            if (p.SearchText!=null)
            {
                query = query.Include(a=>a.ProductAges).Where(a => a.ModelCode.Contains(p.SearchText));
            }
            if (p.ay1!=null&&p.ay1!=0)
            {
            
                //query = query.Where(a => a.Age ==(Age)p.ay1.Value).Include(a => a.ProductAges);
                query = query.Include(a => a.ProductAges).Where(a=>a.ProductAges.Id==p.ay1);
            }  
          
            //if (p.ay2 != null)
            //{
            //    query = query.Where(a => a.Age == (Age)p.ay2.Value);
            //}
            //if (p.ay3!=null)
            //{
            //    query = query.Where(a => a.Age == (Age)p.ay3.Value);
            //}
            //if (p.ay4 != null)
            //{
            //    query = query.Where(a => a.Age == (Age)p.ay4.Value);
            //}
            if (p.gender1 != null)
            {
                query = query.Include(a => a.ProductAges).Where(a => a.Gender == (Gender)(int)p.gender1.Value);
            }
            bool sonuc = p.ay1 == 0 ? true : p.ay1 == null ? true : false;
            if (p.SearchText==null&&sonuc&&p.gender1==null)
            {
                query = query.Include(a => a.ProductAges).Where(a => a.Id != null);
            }
            if (p.Tenant != null && p.Tenant != "Seçim Yapınız")
            {

                query = query.Where(a => a.Barcode == p.Tenant);

            }
            //if (p.gender2 != null)
            //{
            //    query = query.Where(a => a.Gender == (Gender)p.gender2.Value);

            //}
            //query = query.Skip(page).Take(size);

            return query/*.Skip(page).Take(size)*/;
        
        }

        public async Task<ShippingProduct> GetTekliAll(long tekliId)
        {
            return await _productRepository.GetTekliAll(tekliId);
        }

        public async Task<Product> GetProductIdByAge(long productId)
        {
            return await _productRepository.GetProductIdByAge(productId);
        }

        public async Task<Product> GetByProductQuery(long productId)
        {
            var a = await  _productRepository.GetQuery().Include(a=>a.ProductAges).FirstOrDefaultAsync(a=>a.Id==productId);
            return a;
        }

        public async Task<List<Product>> TekliList()
        {
            var a = await _productRepository.GetQuery().Include(a => a.ProductAges).ToListAsync();
            return a; 
        }
    }
}

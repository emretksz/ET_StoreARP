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
using static Entities.Enums.Enums;

namespace DataAccess.Repositories
{
    public class ShippingRepository : Repository<Shippings, ET_StoreARP>, IShippingDal
    {
        //public ShippingRepository(ET_StoreARP context):base(context)
        //{

        //}

        /******************** Kullanmıyorum fromlar örnek olur kalsın *********************************/
        public async Task<List<ShippingListDto>> GetAllShippigDto() 
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {

                var result = await (from s in context.Shippings
                                    join c in context.ShippingDetails on s.Id equals c.ShippinbgId
                                    where s.Id==c.ShippinbgId
                                    select new ShippingListDto
                                    {
                                        //TenantId=s.Tenant.Id,
                                        RegisterDate=s.RegisterDate,
                                        ShippingStasus=s.ShippingStasus,
                                        IsComplated=s.IsComplated,
                                         ShippingCount=s.ShippingCount/*,Price=s.Price,*/
                                    }).ToListAsync();



            }
            return null;
        }



        /*********************///Sipariş detayının içindeki ürün renk ve sipariş  miktarı*//////////////////
        /***********************  Aynı olan produtları engellemek için *****************************/
        public async Task<List<ShippingDetails>> IndexShipping(long id)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var shippingContext =await context.ShippingDetails.Include(a => a.Products).Include(a => a.Shippings).Include(a => a.Color).Where(a=>a.ShippinbgId==id&&a.ShippingCount>0).ToListAsync();
                List<ShippingDetails> detay = new List<ShippingDetails>();
                foreach (var item in shippingContext)
                {
                    if (detay.Count==0)
                    {
                        detay.Add(item);
                    }
                    else
                    {
                        var query = detay.FirstOrDefault(a => a.ProductId == item.ProductId);
                     
                        if (query == null)
                        {
                       
                            detay.Add(item);
                        }
                        else
                        { 
                           var countTemp = query.ShippingCount;
                            detay.Remove(query);
                            item.ShippingCount += countTemp;
                            detay.Add(item);
                        }
                    }
                }
            
                return detay;
            }
        }

        //******* Siparişler sayfasında tüm siparişleri getirmek için*///////////
        public async Task <List<IndexShippingForJson>> IndexIcınGetir()
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {

                var result = await context.Shippings.Include(a => a.Tenant).ToListAsync();/*Where(a=>a.IsComplated ==false*//*&&Convert.ToInt64(a.ShippingCount) >0*///).ToListAsync();/*.(a => a.Id == id);*/
                //var result2 = await context.Shippings.Include(a => a.Tenant).Where(a=>a.IsComplated==true).ToListAsync();/*.(a => a.Id == id);*/
                List<IndexShippingForJson> list = new List<IndexShippingForJson>();
                foreach (var item in result)
                {
                    IndexShippingForJson temp = new IndexShippingForJson();
                    temp.Id=item.Id;
                    temp.ShippingCount = item.ShippingCount;
                    temp.RegisterDate= item.RegisterDate;
                    temp.SiparisAdi = item.SiparisAdi;
                    temp.SiparisTutari=item.SiparisTutari;
                    temp.TenantName = item.Tenant.TenantName!=null? item.Tenant.TenantName:"";
                    temp.ShippingStasus = item.ShippingCount=="0"? ShippingStasus.Tamamlandı.ToString():((ShippingStasus)(int)item.ShippingStasus).ToString();
                    
                    list.Add(temp);
                }
                return list.OrderBy(a=>a.RegisterDate).ToList();
            }
        }
        public async Task<List<Shippings>> IndexIcınGetirMagaza()
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {

                var result = await context.Shippings.Include(a => a.Tenant).Where(a => a.IsComplated == true&& Convert.ToInt64(a.ShippingCount) > 0).ToListAsync();/*.(a => a.Id == id);*/
                //var result2 = await context.Shippings.Include(a => a.Tenant).Where(a=>a.IsComplated==true).ToListAsync();/*.(a => a.Id == id);*/
                return result;
            }
        }


        //********** Gönderdiğim sipariş ıdsindeki product için   onun içine renk ve miktarı Modal fade içinde particial Kullandım bastım  *///////////
        public async Task<List<ShippingProduct>> SiparisİicndekiRenkveMiktarlari(long shippingId,long productId)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var renkveMiktar = await (from a in context.ShippingDetails.Include(a => a.Products).Include(a => a.Shippings).Include(a => a.Color).Where(a => a.ShippinbgId == shippingId &&a.ProductId== productId&&a.ShippingCount>0)
                                  select new ShippingProduct()
                                  {
                                      ColorId = a.Color.Id,
                                      ColorName = a.Color.ColorName,
                                      Count = a.ShippingCount.Value,
                                      ProductId = a.ProductId,
                                      ProductName = a.Products.ModelName,
                                      ShippingId = shippingId

                                  }).ToListAsync();
                return renkveMiktar;
            }
        }     
        
        public async Task<List<ShippingDetails>> UpdateShippingAll(long shippingId)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var siparis = await (from a in context.ShippingDetails.Include(a => a.Products).Include(a => a.Shippings).Where(a => a.ShippinbgId == shippingId )
                                  select new ShippingDetails()
                                  {
                                      Products = a.Products,
                                      ProductId=a.ProductId,
                                      Color = a.Color,
                                      ShippingCount = a.ShippingCount.Value,
                                      Amount = a.Amount,
                                      Price = a.Price,
                                      Shippings = a.Shippings,  
                                  }).ToListAsync();
                return siparis;
            }
        }



    }
}

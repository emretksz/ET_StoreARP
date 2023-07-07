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
    public class OrderDateRepository:Repository<OrderDate, ET_StoreARP>,IOrderDateDal
    {


        public async Task<List<OrderPriceAndProduct>> GetMagazaFaturalari(long ? orderDate=null)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                if (orderDate==null)
                {
                    var result = await (from a in context.Orders
                                        join b in context.Shippings on a.ShippingId equals b.Id
                                        join c in context.Colors on a.ColorId equals c.Id
                                        join d in context.Products on a.ProductId equals d.Id
                                        join e in context.OrderDates on a.OrderDateId equals e.Id
                                        join f in context.Tenants on b.TenantId equals f.Id
                                        select new OrderPriceAndProduct
                                        {
                                            ProductName=d.ModelName,
                                            Age=d.Age,
                                            Barcode=d.Barcode,
                                            ColorName=c.ColorName,
                                            Count=Convert.ToInt64(a.ProductCount),
                                            Gender=d.Gender,
                                            ModelCode=d.ModelCode,
                                            OrderDateId=a.OrderDateId,
                                            OrderId=a.Id,
                                            ShippingId=a.ShippingId,
                                            TenantName=f.TenantName,
                                            OrderTarih=a.RegisterDate.Date.ToString(),
                                            TotalPrice=a.TotalPrice
                                        }).OrderByDescending(a => a.OrderDateId).ToListAsync();

                    List<string> Ids = new List<string>();
                    List<OrderPriceAndProduct> listem = new List<OrderPriceAndProduct>();

                    foreach (var item in result)
                    {
                        if (listem.Count==0)
                        {
                            listem.Add(item);
                            Ids.Add(item.OrderDateId.ToString());
                        }
                        if (!Ids.Contains(item.OrderDateId.ToString()))
                        {
                            listem.Add(item);
                            Ids.Add(item.OrderDateId.ToString());
                        }
                    }
                    return listem;
                }
                else
                {
                    var result = await (from a in context.Orders
                                        join b in context.Shippings on a.ShippingId equals b.Id
                                        join c in context.Colors on a.ColorId equals c.Id
                                        join d in context.Products on a.ProductId equals d.Id
                                        join e in context.OrderDates on a.OrderDateId equals e.Id
                                        join f in context.Tenants on b.TenantId equals f.Id
                                        where  a.OrderDateId==orderDate.Value
                                        select new OrderPriceAndProduct
                                        {
                                            ProductName = d.ModelName,
                                            Age = d.Age,
                                            Barcode = d.Barcode,
                                            ColorName = c.ColorName,
                                            Count = Convert.ToInt64(a.ProductCount),
                                            Gender = d.Gender,
                                            ModelCode = d.ModelCode,
                                            OrderDateId = a.OrderDateId,
                                            OrderId = a.Id,
                                            ShippingId = a.ShippingId,
                                            TenantName = f.TenantName,
                                              OrderTarih = a.RegisterDate.Date.ToString(),
                                            TotalPrice = a.TotalPrice
                                        }).OrderByDescending(a => a.OrderDateId).ToListAsync();
                    return result;
                }
             

            }

        }
        public async Task<List<OrderPriceAndProduct>> GetFabrikaFaturalari(long? orderDate = null)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                if (orderDate == null)
                {
                    var result = await (from a in context.Orders
                                        join b in context.Shippings on a.ShippingId equals b.Id
                                        join c in context.Colors on a.ColorId equals c.Id
                                        join d in context.Products on a.ProductId equals d.Id
                                        join e in context.OrderDates on a.OrderDateId equals e.Id
                                        join f in context.Tenants on b.TenantId equals f.Id
                                       
                                        select new OrderPriceAndProduct
                                        {
                                            ProductName = d.ModelName,
                                            Age = d.Age,
                                            Barcode = d.Barcode,
                                            //BirimFiyati = a.UnitePrice,
                                            ColorName = c.ColorName,
                                            Count = Convert.ToInt64(a.ProductCount),
                                            Gender = d.Gender,
                                            ModelCode = d.ModelCode,
                                            OrderDateId = a.OrderDateId,
                                            OrderId = a.Id,
                                            //Price = a.Price,
                                            ShippingId = a.ShippingId,
                                            TenantName = f.TenantName,
                                            OrderTarih = a.RegisterDate.Date.ToString(),
                                            TotalPrice = a.TotalPrice
                                        }).OrderByDescending(a => a.OrderDateId).ToListAsync();

                    List<string> Ids = new List<string>();
                    List<OrderPriceAndProduct> listem = new List<OrderPriceAndProduct>();

                    foreach (var item in result)
                    {
                        if (listem.Count == 0)
                        {
                            listem.Add(item);
                            Ids.Add(item.OrderDateId.ToString());
                        }
                        if (!Ids.Contains(item.OrderDateId.ToString()))
                        {
                            listem.Add(item);
                            Ids.Add(item.OrderDateId.ToString());
                        }
                        
                    }
                    return listem;
                }
                else
                {
                    var result = await (from a in context.Orders
                                        join b in context.Shippings on a.ShippingId equals b.Id
                                        join c in context.Colors on a.ColorId equals c.Id
                                        join d in context.Products on a.ProductId equals d.Id
                                        join e in context.OrderDates on a.OrderDateId equals e.Id
                                        join f in context.Tenants on b.TenantId equals f.Id
                                        where /*b.MagazaMi == null &&*/ a.OrderDateId == orderDate.Value
                                        select new OrderPriceAndProduct
                                        {
                                            ProductName = d.ModelName,
                                            Age = d.Age,
                                            Barcode = d.Barcode,
                                            //BirimFiyati = a.UnitePrice,
                                            ColorName = c.ColorName,
                                            Count = Convert.ToInt64(a.ProductCount),
                                            Gender = d.Gender,
                                            ModelCode = d.ModelCode,
                                            OrderDateId = a.OrderDateId,
                                            OrderId = a.Id,
                                           // Price = a.Price,
                                            ShippingId = a.ShippingId,
                                            TenantName = f.TenantName,
                                            OrderTarih = a.RegisterDate.Date.ToString(),
                                            TotalPrice = a.TotalPrice
                                        }).OrderByDescending(a=>a.OrderDateId).ToListAsync();
                    return result;
                }


            }


        }
    }
}

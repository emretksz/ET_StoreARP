using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Dtos
{
    public class ProductColorDto
    {
       public List<Color> ProductColor { get; set; }
        public List<Product> Products { get; set; }
        public List<ShippingProduct> ShippingProduct{ get; set; }
        public Product SingleProduct { get; set; }
        
    }
    public class ShippingProduct
    {
        public long TempId { get; set; }
        public long ShippingDetailsId { get; set; }
        public string Barcode { get; set; }
        public long TekliId { get; set; }
        public long ShippingId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public long ColorId { get; set; }
        public string ColorName { get; set; }
        public long Count { get; set; }
        public string Price { get; set; }
        public string BirimFiyati { get; set; }
        public Gender Gender { get; set; }
        public string Age { get; set; }
        public string ModelCode { get; set; }
        public string TenantName { get; set; }
        public long TenantId { get; set; }
       public List<ShippingList> ShippingLists { get; set; }
        public string RenkBarcode { get; set; }
        public long TotalCount { get; set; }
        public long StockCount { get; set; }


    }
    public class ShippingList
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public long ColorId { get; set; }
        public string ColorName { get; set; }
        public long Count { get; set; }
        public Gender Gender { get; set; }
        public Age Age { get; set; }
        public string ModelCode { get; set; }
        public string ImageURL { get; set; }


    }

    public class OrderShippingDto
    {
        public long ProductId { get; set; }
        public string TenantName { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public OrderColorDto ColorDto { get; set; }
        public Gender Gender { get; set; }
        public string Age { get; set; }
        public string ModelCode { get; set; }
        public string ImageURL { get; set; }
        public string RegisterDate { get; set; }
        public string RenkBarcode { get; set; }
        public decimal Price { get; set; }
        public long Amount { get; set; }
        public string Renk { get; set; }
        public long  ColorId { get; set; }
        public long  SpId { get; set; }



    }
    public class OrderColorDto
    {
        public long ColorId { get; set; }
        public string ColorName { get; set; }
        public long  Count { get; set; }


    }
}

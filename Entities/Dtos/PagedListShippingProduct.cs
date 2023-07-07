using Entities.Concrete;
using PagedList;
using System.Collections.Generic;

namespace Entities.Dtos
{
    public class PagedListShippingProduct
    {
        public IPagedList<Color> ProductColor { get; set; }
        public List<Color> ProductColorr { get; set; }
        public  IPagedList<Product> Products { get; set; }
        public List<ShippingProduct> ShippingProduct { get; set; }
        public Product SingleProduct { get; set; }
        public int pageCount { get; set; }

    }
}

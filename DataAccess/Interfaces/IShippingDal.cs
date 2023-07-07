using Core.DataAccess.EntityFramework.Repository;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface  IShippingDal: IRepository<Shippings>
    {
        public Task< List<ShippingListDto>> GetAllShippigDto();
        public Task<List<ShippingDetails>> IndexShipping(long id);
        public Task<List<IndexShippingForJson>> IndexIcınGetir();
        public Task<List<Shippings>> IndexIcınGetirMagaza();
        public Task<List<ShippingProduct>> SiparisİicndekiRenkveMiktarlari(long shippingId, long productId);
        public Task<List<ShippingDetails>> UpdateShippingAll(long shippingId);
    }
}

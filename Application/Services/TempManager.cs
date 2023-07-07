using Application.Interface;
using AutoMapper;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities.Concrete;
using Entities.Dtos;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TempManager : ITempServices
    {

        //private readonly IProductDal _productRepository;
        private readonly StockManager _stockManager = new StockManager(new StockRepository());
        private readonly ColorManager _colorManager = new ColorManager(new ColorRepository());
        // private readonly IMapper _mapper;
        private readonly ITempDal _tempRepository;
        private readonly ITenantDal _tenantRepository;
        private readonly IStockDal _stockRepository;
        private readonly IShippingDetails _shippingDetailsRepository;
        private readonly IShippingDal _shippingRepository;


        public TempManager( /*IMapper mapper, */ITempDal tempRepository, ITenantDal tenantRepository, IStockDal stockRepository, IShippingDetails shippingDetailsRepository, IShippingDal shippingRepository)
        {
            //_mapper = mapper;
            _tempRepository = tempRepository;
            _tenantRepository = tenantRepository;
            _stockRepository = stockRepository;
            _shippingDetailsRepository = shippingDetailsRepository;
            _shippingRepository = shippingRepository;
        }
        public async Task<IResult> Add(Temp temp)
        {
            try
            {
                if (temp != null)
                {
                    await _tempRepository.CreateAsync(temp);
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<IResult> Delete(Temp temp)
        {
            try
            {
                if (temp != null)
                {
                    _tempRepository.Remove(temp);
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<IDataResults<List<Temp>>> GetAll(Expression<Func<Temp, bool>> filter = null)
        {
            if (filter != null)
            {
                return new SuccessDataResult<List<Temp>>(await _tempRepository.GetAllAsync(filter), "Depo Eklendi");

            }
            return new SuccessDataResult<List<Temp>>(await _tempRepository.GetAllAsync(), "Depo Eklendi");
        }

        public async Task<IDataResults<Temp>> GetById(long tempId)
        {
            if (tempId != 0)
            {
                return new SuccessDataResult<Temp>(await _tempRepository.GetByFilterAsync(a => a.Id == tempId));
            }
            return new ErrorDataResult<Temp>("Depo bulunamadı");
        }

        public async Task<List<TempIndexDto>> TempIndex()
        {
            return await _tenantRepository.TempIndex();
        }
        public async Task<List<TempIndexDto>> TenantShippingList(long tenantId)
        {
            return await _tenantRepository.TenantShippingList(tenantId);
        }
        public async Task<List<TempIndexDto>> TempMagazaIndex()
        {
            return await _tenantRepository.TempMagazaIndex();
        }

        public async Task<List<OrderShippingDetailsDto>> OrderShippingProducts(long shippinId)
        {
            return await _tenantRepository.OrderShippingProducts(shippinId);
        }
        public async Task<IResult> Update(Temp temp)
        {
            try
            {
                var tempRepo = await _tempRepository.GetByFilterAsync(a => a.Id == temp.Id);
                _tempRepository.Update(temp, tempRepo);
                return new SuccessResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<IResult> UpdateAll(Temp temp)
        {
            try
            {
                _tempRepository.UpdateAll(temp);
                return new SuccessResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<List<ShippingProduct>> GetParticalShipping(long shippingId)
        {
            return await _tempRepository.GetParticalShipping(shippingId);
        }
        public async Task<List<ShippingProduct>> GetOrderShippingResult(long shippingId)
        {

            try
            {
                return await _tempRepository.GetOrderShippingResult(shippingId);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<List<ShippingProduct>> GetTenantShippingOrderPrice(long? tenantId = null, long? shippingId = null)
        {

            return await _tempRepository.GetTenantShippingOrderPrice(tenantId, shippingId);
        }

        public async Task<List<OrderShippingDetailsDto>> OrderShippingProductsMagaza(long shippinId)
        {
            return await _tenantRepository.OrderShippingProductsMagaza(shippinId);

        }

        public async Task<List<ShippingProduct>> GetTenantShippingOrderPriceMagaza(long? tenantId = null, long? shippingId = null)
        {
            return await _tempRepository.GetTenantShippingOrderPriceMagaza(tenantId, shippingId);
        }

        public async Task<List<Product>> GetPrepareShipping(long shippingId)
        {
            return await _tempRepository.GetPrepareShipping(shippingId);
        }

        public async Task<List<ColorDtoForShipping>> GetPrepareColorList(long shippingId, long productId)
        {
            return await _tenantRepository.GetPrepareColorList(shippingId, productId);
        }

        public Task<List<GetShippingOrderList>> GetShippingList(long shippingId)
        {
            return _tempRepository.GetShippingList(shippingId);
        }

        public Task<ComplatedShippingExcel> ComplatedShippingForExcel(long colorId, long productId)
        {
            return _tempRepository.ComplatedShippingForExcel(colorId, productId);
        }

        public async Task<List<GetShippingOrderList>> GetFis(long shippingId)
        {
            return await _tempRepository.GetFis(shippingId);
        }
        public async Task<List<Shippings>> GetShippingsAsyncForGrid()
        {
            return await _tempRepository.GetShippingsAsyncForGrid();
        }

        public async Task<List<ShippingProduct>> UpdateFinishShippingListView(long spId)
        {
           return await _tempRepository.UpdateFinishShippingListView(spId);
        }  
        public async Task<bool> UpdateFinishShippingPost(List<ShippingProduct> list)
        {
            try
            {
                decimal dusulecekSonMiktar =0;
                foreach (var item in list)
                {
                    long eklenecekMiktar = 0;
                    var temps = await _tempRepository.GetByFilterAsync(a => a.Id == item.TempId);
                    //if (temps.Count == item.Count)
                    //{
                    //    continue;
                    //}
                    if (temps.Count < item.Count)
                    {
                        //eklenen ürün var olan miktardan fazlaysa tempId eşit olan eklenen kolunun  miktarını güncelle
                        //eklenecekMiktar = item.Count-temps.Count ;//bu işlemi eklenen ürün miktarını bulmak için yapıyorum!!!!!!!!

                        ////temp count artık view sayfasından gönderilen kadar
                        //temps.Count = item.Count;/// yüksek miktarla güncelliyorum
                        ////güncellee
                        //_tempRepository.UpdateAll(temps);

                        ////ürün stogu bulundu
                        //var stock = await _stockRepository.GetByFilterAsync(a => a.ColorId == item.ColorId && a.ProductId == item.ProductId);

                        ////eğer ki miktar siparişin normalinden fazlaysa burada miktar artışı olmuş demekttir stoktan düşülmeli
                        //stock.StockCount = stock.StockCount - eklenecekMiktar;
                        ////stok güncellendi
                        //_stockRepository.UpdateAll(stock);
                        //var qq = item.ShippingId;
                        //var sp = await _shippingDetailsRepository.GetByFilterAsync(a => a.ShippinbgId == qq);
                        //sp.ShippingCount = sp.ShippingCount + eklenecekMiktar;
                        //_shippingDetailsRepository.UpdateAll(sp);
                    }
                    else
                    {
                        if (temps.Count >= item.Count)
                        {
                            long dusulecekMiktar = temps.Count - item.Count;

                            temps.Count = dusulecekMiktar;

                            _tempRepository.UpdateAll(temps);

                            var stock = await _stockRepository.GetByFilterAsync(a => a.ColorId == item.ColorId && a.ProductId == item.ProductId);
                            //eğerki siparişten miktarı düşürdüysem bu stoğuma eklemem anlamına gelir
                            stock.StockCount = stock.StockCount  +item.Count;
                            _stockRepository.UpdateAll(stock);
                            var qq = item.ShippingId;
                            var sp =await _shippingDetailsRepository.GetByFilterAsync(a => a.ShippinbgId == qq&&a.ProductId==item.ProductId);
                            //miktar fiyatı
                            var spPriceAmount = sp.Price / sp.ShippingCount;
                            sp.ShippingCount = sp.ShippingCount - item.Count;
                            //yeni price
                            var spsonuc = sp.ShippingCount * spPriceAmount;

                            var oldPrice= sp.Price;
                            sp.Price = spsonuc.Value;
                            _shippingDetailsRepository.UpdateAll(sp);
                            
                            
                            dusulecekSonMiktar += oldPrice - spsonuc.Value;

                         


                        }
                    }
                }

                var shipping = await _shippingRepository.GetByFilterAsync(a => a.Id == list.FirstOrDefault().ShippingId);
                var newPrice = Convert.ToDecimal(shipping.SiparisTutari) - dusulecekSonMiktar;
                shipping.SiparisTutari = newPrice.ToString();
                _shippingRepository.UpdateAll(shipping);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
    }
}

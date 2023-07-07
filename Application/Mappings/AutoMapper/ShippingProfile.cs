using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.AutoMapper
{
    public class ShippingProfile:Profile
    {
        public ShippingProfile()
        {
            CreateMap<Shippings, ShippingListDto>().ReverseMap();
            CreateMap<ShippingListDto, Shippings>().ReverseMap();
        }
    }
}

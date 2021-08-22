using AutoMapper;
using BuyANZCoupon.Dto;
using BuyANZCoupon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Profiles
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponDto>();
            CreateMap<CouponCreateDto, Coupon>()
                .ForMember
                (
                    dest => dest.CodeType,
                    opt => opt.MapFrom(f => f.CodeType.ToLower().Equals("fixed") ? CodeType.Fixed : CodeType.Random)
                );
            CreateMap<CouponUpdateDto, Coupon>();
            CreateMap<Coupon, CouponUpdateDto>();
        }
    }
}

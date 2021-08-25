using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BuyANZCoupon.Models;
using BuyANZCoupon.Services;
using BuyANZCoupon.Dto;
using AutoMapper;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using System.Diagnostics;
using BuyANZCoupon.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BuyANZCoupon.Controllers
{
    [Route("/api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _CouponRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public CouponController
            (
                ICouponRepository couponRepository, 
                IMapper mapper,
                IUrlHelperFactory urlHelperFactory,
                IActionContextAccessor actionContextAccessor
            )
        {
            _CouponRepository = couponRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        [HttpGet("forUser/{userId}")]
        public async Task<IActionResult> GetAllValidCoupons
            (
                [FromRoute]string userId,
                [FromQuery]CouponFilterParameters couponFilterParameters,
                [FromQuery]PaginationParameters paginationParas
            )
        {
            var couponsFromRepo = await _CouponRepository.GetAllValidCouponsAsync(
                                                                userId, 
                                                                couponFilterParameters.Operator, 
                                                                couponFilterParameters.FilterValue, 
                                                                paginationParas.PageNumber, 
                                                                paginationParas.PageSize);

            if (couponsFromRepo == null || couponsFromRepo.Count() <= 0)
            {
                return NotFound("No coupons have been found for this user");
            }

            var couponsDtos = _mapper.Map<IEnumerable<CouponDto>>(couponsFromRepo);

            var previousPageLink = couponsFromRepo.HasPrevious ?
                                     GenerateTouristRouteResourceURL(couponFilterParameters, paginationParas, "prev") : 
                                     null;
            
            var nextPageLink = couponsFromRepo.HasNext ?
                                     GenerateTouristRouteResourceURL(couponFilterParameters, paginationParas, "next") :
                                     null;

            // X-Pagination
            var paginationMetaData = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = couponsFromRepo.TotalCount,
                pageSize = couponsFromRepo.PageSize,
                pageNumber = couponsFromRepo.PageNumber,
                totalPages = couponsFromRepo.TotalPages
            };

            Response.Headers.Add("x-pagination", 
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));

            return Ok(couponsDtos);
        }

        [HttpGet("{couponId}")]
        public async Task<IActionResult> GetCouponById([FromRoute]string couponId)
        {
            if (! await _CouponRepository.IsCouonExists(couponId))
            {
                return BadRequest("Coupon was not found");
            }

            return Ok(await _CouponRepository.GetCouponById(couponId));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpPost("{validDays}", Name = "GenerateCoupon")]
        public async Task<IActionResult> GenerateCoupon
            (
                [FromBody]CouponCreateDto couponCreateDto,
                int validDays = 7
            )
        {
            if (couponCreateDto == null)
            {
                return BadRequest("Coupon object needed");
            }

            var newCoupon = _mapper.Map<Coupon>(couponCreateDto);

            // Check if the coupon code is null when generating Random CodeType coupon
            if (couponCreateDto.CodeType == null || newCoupon.CodeType == CodeType.Fixed && newCoupon.CouponCode == null)
            {
                return BadRequest("CodeType cannot be null or coupon code cannot be null when generating Random Type Coupon");
            }

            newCoupon.ExpirationDate = DateTime.UtcNow.AddDays(validDays);

            // When CodeType is Random, generate the random code
            if (newCoupon.CodeType == CodeType.Random)
            {
                newCoupon.CouponCode = _CouponRepository.RandomCouponCode();
            }

            _CouponRepository.AddCoupon(newCoupon);
            await _CouponRepository.SaveAsync();

            var couponToReturn = _mapper.Map<CouponDto>(newCoupon);

            return Ok(couponToReturn);
        }

        [HttpPut("{couponId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateCouponById
            (
                [FromRoute]string couponId,
                [FromBody]JsonPatchDocument<CouponUpdateDto> patchDoc
            )
        {
            if (! await _CouponRepository.IsCouonExists(couponId))
            {
                return BadRequest("Coupon was not found");
            }

            if (patchDoc == null)
            {
                return BadRequest("Please specify the property need to be updated according to JsonPatch documentation");
            }

            var couponFromRepo = await _CouponRepository.GetCouponById(couponId);
            var couponToPatch = _mapper.Map<CouponUpdateDto>(couponFromRepo);
            patchDoc.ApplyTo(couponToPatch, ModelState);
            if (!TryValidateModel(couponToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(couponToPatch, couponFromRepo);
            await _CouponRepository.SaveAsync();

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpPost("assignTo/{userId}/couponId/{couponId}")]
        public async Task<IActionResult> AssignCoupon
            (
                [FromRoute]string userId,
                [FromRoute]string couponId
            )
        {

            if (!await _CouponRepository.IsCouonExists(couponId))
            {
                return NotFound("The coupon was not found");
            }

            if (!await _CouponRepository.IsUserExists(userId))
            {
                return NotFound("The user was not found");
            }


            var couponFromRepo = await _CouponRepository.GetCouponById(couponId);
            var userFromRepo = await _CouponRepository.GetUserById(userId);

            couponFromRepo.User = userFromRepo;

            if (userFromRepo.Coupons == null)
            {
                userFromRepo.Coupons = new Collection<Coupon>();
            }

            userFromRepo.Coupons.Add(couponFromRepo);

            await _CouponRepository.SaveAsync();

            var couponDto = _mapper.Map<CouponDto>(couponFromRepo);

            return Ok(couponDto);
        }

        private string GenerateTouristRouteResourceURL
            (
                CouponFilterParameters paras1,
                PaginationParameters paras2,
                string type
            )
        {
            return type switch
            {
                "prev" => _urlHelper.Link("forUser",
                new
                {
                    Price = paras1.Operator + paras1.FilterValue,
                    pageNumber = paras2.PageNumber - 1,
                    pageSize = paras2.PageSize
                }),
                "next" => _urlHelper.Link("",
                new
                {
                    Price = paras1.Operator + paras1.FilterValue,
                    pageNumber = paras2.PageNumber + 1,
                    pageSize = paras2.PageSize
                }),
                _ => _urlHelper.Link("",
                new
                {
                    Price = paras1.Operator + paras1.FilterValue,
                    pageNumber = paras2.PageNumber - 1,
                    pageSize = paras2.PageSize
                })
            };
        }
    }
}

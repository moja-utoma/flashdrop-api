using AutoMapper;
using Flashdrop.Application.Products.DTOs.Requests;
using Flashdrop.Application.Products.DTOs.Response;
using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Sales;

public class SalesProfile : Profile
{
    public SalesProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductForListDto>();
        CreateMap<CreateProductRequest, Product>()
            .ValidateMemberList(MemberList.Source);
        CreateMap<UpdateProductRequest, Product>()
            .ValidateMemberList(MemberList.Source);
    }
}
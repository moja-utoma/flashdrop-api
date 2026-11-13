using AutoMapper;
using Flashdrop.Application.Products.DTOs.Requests;
using Flashdrop.Application.Products.DTOs.Response;
using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Application.Sales.DTOs.Responses;
using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Sales;

public class SalesProfile : Profile
{
    public SalesProfile()
    {
        CreateMap<Sale, SaleDto>();
        CreateMap<Sale, SaleForListDto>();
        CreateMap<CreateSaleRequest, Sale>()
            .ValidateMemberList(MemberList.Source);
        CreateMap<UpdateSaleRequest, Sale>()
            .ValidateMemberList(MemberList.Source);
    }
}
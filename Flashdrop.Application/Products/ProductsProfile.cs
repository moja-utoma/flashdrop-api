using AutoMapper;
using Flashdrop.Application.Products.DTOs.Requests;
using Flashdrop.Application.Products.DTOs.Response;
using Flashdrop.Data.Entities;

namespace Flashdrop.Application.Products;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductForListDto>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();
    }
}

using AutoMapper;
using BetashipEcommerce.APP.Queries.Customers.GetCustomerById;
using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using BetashipEcommerce.APP.Queries.Products.GetProductById;
using BetashipEcommerce.APP.Queries.Products.GetProductList;
using BetashipEcommerce.APP.Queries.Products.SearchProducts;
using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Orders.Entities;
using BetashipEcommerce.CORE.Products;

namespace BetashipEcommerce.APP.Common.Mappings;

/// <summary>
/// AutoMapper profile for mapping domain entities to DTOs.
/// </summary>
public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.LastModifiedAt, o => o.MapFrom(s => s.UpdatedAt));

        CreateMap<Product, ProductListDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Product, ProductSearchResultDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.Value))
            .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId.Value))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.TotalAmount.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.TotalAmount.Currency))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId.Value))
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice.Amount))
            .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.TotalPrice.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.UnitPrice.Currency));

        // Customer mappings
        CreateMap<Customer, CustomerDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Value))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber != null ? s.PhoneNumber.Value : null))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Addresses, o => o.MapFrom(s => s.Addresses));
    }
}

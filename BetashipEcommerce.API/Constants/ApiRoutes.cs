namespace BetashipEcommerce.API.Constants;

public static class ApiRoutes
{
    private const string Base = "api";

    public static class Products
    {
        private const string Prefix = $"{Base}/products";
        public const string Create        = Prefix;
        public const string GetById       = $"{Prefix}/{{productId:guid}}";
        public const string GetList       = Prefix;
        public const string Update        = $"{Prefix}/{{productId:guid}}";
        public const string UpdatePrice   = $"{Prefix}/{{productId:guid}}/price";
        public const string Publish       = $"{Prefix}/{{productId:guid}}/publish";
        public const string Discontinue   = $"{Prefix}/{{productId:guid}}/discontinue";
        public const string Search        = $"{Prefix}/search";
    }

    public static class Orders
    {
        private const string Prefix = $"{Base}/orders";
        public const string Place         = Prefix;
        public const string GetById       = $"{Prefix}/{{orderId:guid}}";
        public const string GetByCustomer = $"{Prefix}/customer/{{customerId:guid}}";
        public const string Confirm       = $"{Prefix}/{{orderId:guid}}/confirm";
        public const string Ship          = $"{Prefix}/{{orderId:guid}}/ship";
        public const string Cancel        = $"{Prefix}/{{orderId:guid}}/cancel";
        public const string GetHistory    = $"{Prefix}/history";
    }

    public static class Carts
    {
        private const string Prefix = $"{Base}/carts";
        public const string Get        = $"{Prefix}/{{customerId:guid}}";
        public const string AddItem    = $"{Prefix}/{{customerId:guid}}/items";
        public const string UpdateItem = $"{Prefix}/{{customerId:guid}}/items/{{productId:guid}}";
        public const string RemoveItem = $"{Prefix}/{{customerId:guid}}/items/{{productId:guid}}";
        public const string Clear      = $"{Prefix}/{{customerId:guid}}";
        public const string Checkout   = $"{Prefix}/{{customerId:guid}}/checkout";
    }

    public static class Customers
    {
        private const string Prefix = $"{Base}/customers";
        public const string Register       = Prefix;
        public const string GetById        = $"{Prefix}/{{customerId:guid}}";
        public const string UpdateProfile  = $"{Prefix}/{{customerId:guid}}";
        public const string AddAddress     = $"{Prefix}/{{customerId:guid}}/addresses";
        public const string GetOrders      = $"{Prefix}/{{customerId:guid}}/orders";
    }

    public static class Payments
    {
        private const string Prefix = $"{Base}/payments";
        public const string Initiate = Prefix;
        public const string Process  = $"{Prefix}/{{paymentId:guid}}/process";
        public const string Confirm  = $"{Prefix}/{{paymentId:guid}}/confirm";
        public const string Refund   = $"{Prefix}/{{paymentId:guid}}/refund";
    }

    public static class Auth
    {
        private const string Prefix = $"{Base}/auth";
        public const string Login        = $"{Prefix}/login";
        public const string Register     = $"{Prefix}/register";
        public const string RefreshToken = $"{Prefix}/refresh";
        public const string Logout       = $"{Prefix}/logout";
        public const string VerifyEmail  = $"{Prefix}/verify-email";
    }

    public static class Admin
    {
        private const string Prefix = $"{Base}/admin";
        public const string Dashboard   = $"{Prefix}/dashboard";
        public const string AuditLogs   = $"{Prefix}/audit-logs";
        public const string Analytics   = $"{Prefix}/analytics";
        public const string ManageUsers = $"{Prefix}/users";
    }
}

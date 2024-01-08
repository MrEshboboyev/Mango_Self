using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// add HttpClient
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// for couponService
builder.Services.AddHttpClient<ICouponService, CouponService>();
// for authService
builder.Services.AddHttpClient<IAuthService, AuthService>();
// for productService
builder.Services.AddHttpClient<IProductService, ProductService>();
// for cartService
builder.Services.AddHttpClient<ICartService, CartService>();


// service lifetime for IBaseService, ICouponService, IAuthService, ITokenProvider, IProductService, ICartService
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

// CouponAPIBase populated
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
// AuthAPIBase populated
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
// ProductAPIBase populated
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
// ShoppingCartAPIBase populated
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];


// add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.ExpireTimeSpan = TimeSpan.FromHours(10);
        option.LoginPath = "/Auth/Login";
        option.AccessDeniedPath = "/Auth/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


// adding pipeline for authentication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

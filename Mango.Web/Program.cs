using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// add HttpClient
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// for couponService
builder.Services.AddHttpClient<ICouponService, CouponService>();


// service lifetime for IBaseService, ICouponService
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();

// CouponAPIBase populated
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
// CouponAPIBase populated
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Http;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register HttpClient and custom handler for API calls
builder.Services.AddTransient<ApiClientAuthHandler>();
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<ApiClientAuthHandler>();

// Configure authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Denied";
    });

// Register api services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<DonationService>();
builder.Services.AddScoped<OutgoingDonationService>();

// ===== SERVICIOS DE VOLUNTARIOS =====
builder.Services.AddScoped<VolunteerRequestService>();
builder.Services.AddScoped<VolunteerHoursService>();
//builder.Services.AddScoped<VolunteerDashboardService>();

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
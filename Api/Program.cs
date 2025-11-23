using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database;
using Api.Database.Repositories;
using Api.Middleware;
using Api.Middlewares;
using Api.Services.Application;
using Api.Services.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Add services to the container.
// --------------------------------------------------

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add configuration for JWT authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };
    });

// Configure Entity Framework and SQL Server with retry on failure
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register in-memory caching
builder.Services.AddMemoryCache();

// Register HTTP client factory
builder.Services.AddHttpClient();

// Add health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IOutgoingDonationService, OutgoingDonationService>();
builder.Services.AddScoped<IVolunteerRequestService, VolunteerRequestService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();
builder.Services.AddScoped<IScholarshipPaymentService, ScholarshipPaymentService>();
builder.Services.AddScoped<IHomeContentService, HomeContentService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// Register infrastructure services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IExchangeRateService, BccrHttpExchangeRateService>();

// Conditional registration of services based on environment
if (builder.Environment.IsProduction())
{
    builder.Services.AddScoped<IEmailService, AzureEmailService>();
}
else
{
    builder.Services.AddScoped<IEmailService, SmtpEmailService>();
}

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
builder.Services.AddScoped<IOutgoingDonationRepository, OutgoingDonationRepository>();
builder.Services.AddScoped<IVolunteerRequestRepository, VolunteerRequestRepository>();
builder.Services.AddScoped<IDonationsRepository, DonationsRepository>();
builder.Services.AddScoped<IFinancialRepository, FinancialRepository>();
builder.Services.AddScoped<IHomeContentRepository, HomeContentRepository>();
builder.Services.AddScoped<INewsletterRepository, NewsletterRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Comentar HTTPS redirection para Azure (puede causar problemas)
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Use custom middlewares
app.UseMiddleware<ModelStateValidationMiddleware>();
app.UseMiddleware<TransactionalMiddleware>();
app.UseMiddleware<UnhandledExceptionMiddleware>();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

await app.RunAsync();
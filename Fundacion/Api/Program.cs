using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database;
using Api.Database.Repositories;
using Api.Middleware;
using Api.Middlewares;
using Api.Services.Application;
using Api.Services.Infrastructure;
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

// CONFIGURACIÓN DE BASE DE DATOS
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Usar connection string por defecto para testing en Azure
    Console.WriteLine("No connection string found, using default");
    connectionString = "Server=(localdb)\\mssqllocaldb;Database=FundacionTest;Trusted_Connection=true;MultipleActiveResultSets=true;";
}

Console.WriteLine("Configuring SQL Server database");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

// Register in-memory caching
builder.Services.AddMemoryCache();

// Registar HTTP client factory
builder.Services.AddHttpClient();

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
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IExchangeRateService, BccrHttpExchangeRateService>();

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

// CONFIGURACIÓN MEJORADA DEL PIPELINE
// Habilitar Swagger en TODOS los entornos (incluyendo Azure)
app.UseSwagger();
app.UseSwaggerUI();

// Comentar HTTPS redirection para Azure (puede causar problemas)
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Use custom middlewares
app.UseMiddleware<ModelStateValidationMiddleware>();
app.UseMiddleware<TransactionalMiddleware>();
app.UseMiddleware<UnhandledExceptionMiddleware>();

app.MapControllers();

// MANEJO SEGURO DE MIGRACIONES
if (!string.IsNullOrEmpty(app.Configuration.GetConnectionString("DefaultConnection")) &&
    Environment.GetEnvironmentVariable("APPLY_MIGRATIONS") == "true")
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            Console.WriteLine("Applying migrations...");
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
        // No fallar la aplicación por errores de migración en el primer intento
    }
}

// CONFIGURACIÓN ESPECÍFICA PARA AZURE
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

await app.RunAsync();
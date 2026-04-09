using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SagaOrchestrator.Application.Common.Settings;
using SagaOrchestrator.Application.Sagas.Interfaces;
using SagaOrchestrator.Application.Sagas.Services;
using SagaOrchestrator.Infrastructure.Clients;
using SagaOrchestrator.Infrastructure.Persistence;
using SagaOrchestrator.Infrastructure.Sagas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinica San Felipe - Saga Orchestrator API",
        Version = "v1",
        Description = "Orquestador Saga para compras y ventas."
    });
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                  ?? throw new InvalidOperationException("La sección Jwt no está configurada.");

builder.Services.AddSingleton(jwtSettings);

builder.Services.AddDbContext<SagaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ISagaRepository, SagaRepository>();
builder.Services.AddScoped<ISaleSagaService, SaleSagaService>();
builder.Services.AddScoped<IPurchaseSagaService, PurchaseSagaService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<ISaleServiceClient, SaleServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:SalesService"]
        ?? throw new InvalidOperationException("Services:SalesService no está configurado."));
});

builder.Services.AddHttpClient<IPurchaseServiceClient, PurchaseServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:PurchaseService"]
        ?? throw new InvalidOperationException("Services:PurchaseService no está configurado."));
});

builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ProductService"]
        ?? throw new InvalidOperationException("Services:ProductService no está configurado."));
});

builder.Services.AddHttpClient<IMovementServiceClient, MovementServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:MovementService"]
        ?? throw new InvalidOperationException("Services:MovementService no está configurado."));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinica San Felipe - Saga Orchestrator API v1");
});

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
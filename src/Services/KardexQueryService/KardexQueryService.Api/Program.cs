using System.Reflection;
using System.Text;
using KardexQueryService.Application.Common.Settings;
using KardexQueryService.Application.Kardex.Interfaces;
using KardexQueryService.Application.Kardex.Services;
using KardexQueryService.Infrastructure.Http;
using KardexQueryService.Infrastructure.Movements;
using KardexQueryService.Infrastructure.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinica San Felipe - KardexQueryService API",
        Version = "v1",
        Description = "Servicio de consultas para Kardex."
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

var downstreamSettings = builder.Configuration.GetSection("DownstreamServices").Get<DownstreamServicesSettings>()
                        ?? throw new InvalidOperationException("La sección DownstreamServices no está configurada.");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(downstreamSettings);

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

builder.Services.AddTransient<ForwardAuthorizationHeaderHandler>();

builder.Services.AddHttpClient<IProductQueryGateway, ProductQueryGateway>((sp, client) =>
{
    var settings = sp.GetRequiredService<DownstreamServicesSettings>();
    client.BaseAddress = new Uri(settings.ProductServiceBaseUrl);
})
.AddHttpMessageHandler<ForwardAuthorizationHeaderHandler>();

builder.Services.AddHttpClient<IMovementQueryGateway, MovementQueryGateway>((sp, client) =>
{
    var settings = sp.GetRequiredService<DownstreamServicesSettings>();
    client.BaseAddress = new Uri(settings.MovementServiceBaseUrl);
})
.AddHttpMessageHandler<ForwardAuthorizationHeaderHandler>();

builder.Services.AddScoped<IKardexQueryService, KardexQueryService.Application.Kardex.Services.KardexQueryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinica San Felipe - KardexQueryService API v1");
});

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
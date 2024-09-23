using CCI.Domain.EF;
using CCI.Domain.Entities;
using CCI.Model.Options;
using CCI.Service;
using CCI.Service.Contractors;
using CCIIdentity;
using CCIIdentity.Configurations;
using CCIIdentity.Middleware;
using CCIIdentity.ModuleRegistrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                        .Build();

var connectionString = builder.Configuration.GetConnectionString("NpgsqlDatabase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("=======Has not define connection string yet!!!=======");
    return;
}

SerilogRegister.Initialize(configuration);

// Add services to the container.
builder.Host.UseSerilog();
// services.AddTransient<TokenManagerMiddleware>();
// services.AddTransient<ITokenManagerService, TokenManagerService>();
// services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//add redis cache
// services.AddDistributedRedisCache(r => { r.Configuration = configuration["redis:connectionString"]; });
services
    .AddOptionCollection(configuration)
    .AddRepositoryCollection(connectionString)
    .AddServiceCollection()
    .AddAutoMapper(typeof(Program));

services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString);
});

services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromMinutes(5));

services.AddIdentity<User, Role>(options =>
                {
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                })
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

services.AddControllers();

services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = new Uri(builder.Configuration.GetValue<string>("IdentityServerUri")).AbsoluteUri;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

#region IdentityServer

var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
services.AddIdentityServer()
       .AddConfigurationStore(options =>
       {
           options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
               sql =>
               {
                   sql.MigrationsAssembly(migrationsAssembly);
                   sql.EnableRetryOnFailure();
               });
       })
       .AddOperationalStore(options =>
       {
           options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                    sql.EnableRetryOnFailure();
                });
           options.EnableTokenCleanup = true;
           options.TokenCleanupInterval = 30;
       })
       .AddDeveloperSigningCredential()
       .AddAspNetIdentity<User>()
       .AddProfileService<ProfileService>()
       .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
#endregion

#region Swagger

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "V1",
        Title = "ITE Connect Identity API",
        Contact = new OpenApiContact
        {
            Name = "ITE Connect Identity"
        }
    });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Enter token into field 'Bearer'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string [] {}
        }
    });
});

#endregion

#region Cors

services.AddCors(setup =>
{
    setup.AddPolicy("Default", corsPolicyBuilder =>
        corsPolicyBuilder
            .WithOrigins("http://210.211.99.111:15060/",
                        "http://210.211.99.111:15160/",
                        "http://210.211.99.111:15071/",
                        "http://localhost:2003/")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

services.AddAuthorization(option =>
                {
                    option.AddPolicy("user_profile", policy =>
                    {
                        policy.RequireClaim("scope", "profile");
                    });

                    option.AddPolicy("admin", policy =>
                    {
                        policy.RequireClaim("scope", "admin");
                        policy.RequireClaim(ClaimTypes.Role, "admin");
                    });

                    option.AddPolicy("admin_recruiter", policy =>
                    {
                        policy.RequireClaim("scope", "admin", "recruiter");
                        policy.RequireClaim(ClaimTypes.Role, "admin", "recruiter");
                    });
                });

            option.AddPolicy("admin_recruiter", policy =>
            {
                policy.RequireClaim("scope", "admin", "recruiter");
                policy.RequireRole("admin", "recruiter");
            });
        });
var jwtSection = configuration.GetSection("jwt");
var jwtOptions = new JwtOptions();
jwtSection.Bind(jwtOptions);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseMiddleware<ErrorHandlerMiddleware>();
// app.UseMiddleware<TokenManagerMiddleware>();

app.UseIdentityServer();

app.UseCors("Default");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

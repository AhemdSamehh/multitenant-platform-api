using Application;
using Application.Features.Identity.Roles;
using Application.Features.Identity.Schools;
using Application.Features.Identity.Tenancy;
using Application.Features.Identity.Tokens;
using Application.Features.Identity.Users;
using ABCShared.Library.Wrappers;
using Finbuckle.MultiTenant;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity;
using Infrastructure.Identity.Auth;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Tokens;
using Infrastructure.OpenApi;
using Infrastructure.Schools;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ABCShared.Library.Constants;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddDbContext<TenantDbContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .AddMultiTenant<ABCSchoolTenantInfo>()
                .WithHeaderStrategy(TenancyConstants.TenantIdName)
                .WithClaimStrategy(TenancyConstants.TenantIdName)
                .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>()
                .Services

                .AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
                .AddTransient<ApplicationDbSeeder>()
                .AddTransient<ITenantService , TenantService>()
                .AddTransient<ISchoolService , SchoolService>()

                .AddIdentityService()
                .AddOPenApiDocumentation(configuration)
                .AddPermission();
        }

        public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider , CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();

            await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>()
                .InitializeDatabaseAsync(cancellationToken);
        }

        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            return services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
                .Services
                .AddScoped<ITokenService , TokenService>()
                .AddScoped<IRoleService , RoleService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ICurrentUSerService, CurrentUserService>()
                .AddScoped<CrrentUserMiddleware>()
                ;
        }

      

        public static IServiceCollection AddPermission(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        public static JwtSettings GetJwtSettings( this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettingsConfig = configuration.GetSection(nameof(JwtSettings));
            services.Configure<JwtSettings>(jwtSettingsConfig);

            return jwtSettingsConfig.Get<JwtSettings>();
        }



        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services , JwtSettings jwtSettings)
        {
            var secret = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            services
                .AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    };

                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";

                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired"));
                                return context.Response.WriteAsync(result);
                            }
                            else
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.ContentType = "application/json";

                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("In unhandled error has occured"));
                                return context.Response.WriteAsync(result);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if(!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";

                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized"));
                                return context.Response.WriteAsync(result);
                            }
                            return Task.CompletedTask;
                        },
                        OnForbidden = context=>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";

                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this response"));
                            return context.Response.WriteAsync(result);
                        }
                    };
                });
            services.AddAuthorization(options =>
            {
                foreach (var prop in typeof(SchoolPermission).GetNestedTypes()
                .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue != null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy
                        .RequireClaim(ClaimConstants.Permission, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }

        public static IServiceCollection AddOPenApiDocumentation(this IServiceCollection services , IConfiguration configuration)
        {
            var swaggerSettings = configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

            services.AddEndpointsApiExplorer();
            _ = services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.PostProcess = doc =>
                {
                    doc.Info.Title = swaggerSettings.Title;
                    doc.Info.Description = swaggerSettings.Description;
                    doc.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = swaggerSettings.ContactName,
                        Email = swaggerSettings.ContactEmail,
                        Url = swaggerSettings.ContactUrl,
                    };
                    doc.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = swaggerSettings.LicenseName,
                        Url = swaggerSettings.LicenseUrl,
                    };
                };

                document.AddSecurity(JwtBearerDefaults.AuthenticationScheme , new NSwag.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter Your Bearer(jwt) token to access this API",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Type = NSwag.OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
                document.OperationProcessors.Add(new SwaggerGlobalAuthProcessor());
                document.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());
            });
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            return app
                .UseAuthentication()
                 .UseMiddleware<CrrentUserMiddleware>()
                .UseMultiTenant()
                .UseAuthorization()
                .UseOpenApiDocumentation();
        }
        public static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi(option =>
            {
                option.DefaultModelExpandDepth = -1;
                option.DocExpansion = "none";
                option.TagsSorter = "alpha";
            });
            return app;
        }
    }
}

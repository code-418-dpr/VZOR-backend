using System.Net.Http.Headers;
using System.Text.Json;
using AspNet.Security.OAuth.Yandex;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VZOR.Accounts.Application;
using VZOR.Accounts.Application.Managers;
using VZOR.Accounts.Domain;
using VZOR.Accounts.Infrastructure.IdentityManagers;
using VZOR.Accounts.Infrastructure.Options;
using VZOR.Accounts.Infrastructure.Seeding;
using VZOR.Core.Common;
using VZOR.Core.Database;
using VZOR.Core.Options;
using VZOR.Framework;
using VZOR.Framework.Authorization;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;

namespace VZOR.Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentityServices(configuration)
            .AddJwtAuthentication(configuration)
            .AddDbContexts()
            .AddAuthorizationServices()
            .AddDatabase();
        
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        
        return services;
    }

    private static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentity<User,Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AccountsDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IPermissionManager, PermissionManager>();
        services.AddScoped<PermissionManager>();
        services.AddScoped<RolePermissionManager>();
        services.AddScoped<AccountManager>();
        services.AddScoped<IAccountManager,AccountManager>();
        services.AddScoped<IRefreshSessionManager, RefreshSessionManager>();

        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.ADMIN));
        services.AddScoped<AccountSeedService>();
        
        return services;
    }

    private static IServiceCollection AddDbContexts(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Constraints.Contexts.AuthContext);
        services.AddScoped<AccountsDbContext>();
        services.AddAsyncInitializer<DbInitializer>();
        
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddKeyedScoped<ISqlConnectionFactory,SqlConnectionFactory>(Constraints.Contexts.AuthContext);

        //Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        return services;
    }
    
    private static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddAuthorization();
        services.AddSingleton<AccountsSeeder>();
        
        return services;
    }
    
    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.AddTransient<ITokenProvider,JwtTokenProvider>();
        
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.JWT));
        
        services.Configure<RefreshSessionOptions>(
            configuration.GetSection(RefreshSessionOptions.REFRESH_SESSION));
        
        services.AddDistributedMemoryCache();
        
        services
            .AddAuthentication(options =>
            {
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme; 
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; 
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.HttpOnly = true;
            })
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>()
                                 ?? throw new ApplicationException("missing jwt options");

                options.TokenValidationParameters =
                    TokenValidationParametersFactory.CreateWithLifeTime(jwtOptions);
            })
            /*.AddYandex(options =>
            {
                options.ClientId = "7b6960e71e8a4a78acff0880aaf0d373"; 
                options.ClientSecret = "82ea36e876104a7f9f2199c04933d248"; 
                options.CallbackPath = new PathString("/api/Account/yandex-callback"); 
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })*/
            .AddOAuth("Yandex", options =>
            {
                options.ClientId = "7b6960e71e8a4a78acff0880aaf0d373";
                options.ClientSecret = "82ea36e876104a7f9f2199c04933d248";
                options.CallbackPath = new PathString("/api/Account/yandex-callback");

                options.AuthorizationEndpoint = "https://oauth.yandex.ru/authorize";
                options.TokenEndpoint = "https://oauth.yandex.ru/token";
                options.UserInformationEndpoint = "https://login.yandex.ru/info";

                options.Scope.Add("login:email");

                options.SaveTokens = true;

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                        context.RunClaimActions(user.RootElement);
                    }
                };
            });;

        return services;
    }
    
}
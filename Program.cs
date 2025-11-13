using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace IdentityOAuth2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // inject connection db
            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseOpenIddict();
            });

            var allowOrigins = builder.Configuration["IdentityServer:AllowOrigins"].Split(',');
            builder.Services.AddCors(o => o.AddPolicy("MSCPolicy", builder =>
            {
                builder.WithOrigins(allowOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                        .AddEntityFrameworkStores<IdentityDbContext>()
                        .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Domain = null;
                options.Cookie.Name = "lht.identity.server";
                options.Cookie.SameSite = SameSiteMode.None; // Cho phép cross-site
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ gửi qua HTTPS
            });


            //// OpenIddict
            builder.Services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<IdentityDbContext>();
                })
                .AddServer(options =>
                {
                    options.SetAuthorizationEndpointUris("/connect/authorize")
                               .SetTokenEndpointUris("/connect/token")
                               .SetUserInfoEndpointUris("/connect/userinfo")
                                //.SetLogoutEndpointUris("/connect/logout")
                                .SetIntrospectionEndpointUris("/connect/introspect")
                           //.AllowPasswordFlow()
                           .AllowRefreshTokenFlow()
                           .AllowClientCredentialsFlow()
                           .AllowAuthorizationCodeFlow()
                           .RequireProofKeyForCodeExchange()

                           .RegisterScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Profile, "api")

                           .AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate()

                           .UseAspNetCore()
                           .EnableAuthorizationEndpointPassthrough()
                           //.EnableTokenEndpointPassthrough()
                           ;
                })
                .AddValidation(opt => { opt.UseLocalServer(); opt.UseAspNetCore(); });


            // External Auth Service
            //builder.Services.AddHttpClient<ExternalAuthService>();

            // Custom authen username/password
            //builder.Services.AddScoped<IUserValidator<IdentityUser>, CustomUserValidator>();

            builder.Services.AddControllers();

            //builder.Services.AddEndpointsApiExplorer(); //??

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //app.UseDeveloperExceptionPage();
            //app.UseStatusCodePagesWithReExecute("/error");

            //app.UseForwardedHeaders();

            app.UseCors("MSCPolicy");

            //app.UseForwardedHeaders();

            app.UseRouting();

            // Expose Prometheus metrics

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
                options.MapDefaultControllerRoute();
            });

            app.Run();
        }
    }
}

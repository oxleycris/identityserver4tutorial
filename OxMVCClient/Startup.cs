using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace OxMVCClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // We’ve turned off the JWT claim type mapping to allow well-known claims (e.g. ‘sub’ and ‘idp’) .
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Add support for OpenID Connect authentication to the MVC application.
            // AddAuthentication adds the authentication services to DI. 
            services.AddAuthentication(options =>
            {
                // We are using a cookie as the primary means to authenticate a user (via "Cookies" as the DefaultScheme).
                options.DefaultScheme = "Cookies";
                // We set the DefaultChallengeScheme to "oidc" because when we need the user to login, we will be using the OpenID Connect scheme.
                options.DefaultChallengeScheme = "oidc";
            })
            // We then use AddCookie to add the handler that can process cookies.
            .AddCookie("Cookies")
            // Finally, AddOpenIdConnect is used to configure the handler that perform the OpenID Connect protocol.
            .AddOpenIdConnect("oidc", options =>
            {
                // SignInScheme is used to issue a cookie using the cookie handler once the OpenID Connect protocol is complete.
                options.SignInScheme = "Cookies";

                // The Authority indicates that we are trusting IdentityServer. 
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;

                // We then identify this client via the ClientId.
                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";
                // And SaveTokens is used to persist the tokens from IdentityServer in the cookie (as they will be needed later).
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("api1");
                options.Scope.Add("offline_access");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // This builds up the pipeline in order.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // To ensure the authentication services execute on each request, add UseAuthentication to Configure.
            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}

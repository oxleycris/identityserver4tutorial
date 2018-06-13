using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OxServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        // In ConfigureServices the required services are configured and added to the DI system. 
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC to ConfigureServices with the AddMvc extension method.
            services.AddMvc();

            // AddIdentityServer registers the IdentityServer services in DI. It also registers an in-memory store for runtime state. 
            // This is useful for development scenarios. For production scenarios you need a persistent or shared store like a database or cache for that. 
            services.AddIdentityServer()
                    // The AddDeveloperSigningCredential extension creates temporary key material for signing tokens. 
                    // Again this might be useful to get started, but needs to be replaced by some persistent key material for production scenarios.
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    // Configure identity server with in-memory stores, keys, clients and resources
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    // Register the test users with IdentityServer.
                    .AddTestUsers(Config.GetUsers());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // In Configure the middleware is added to the HTTP pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();

            // Add MVC as the last middleware in the pipeline in Configure with the UseMvc extension method.
            app.UseMvcWithDefaultRoute();
        }
    }
}

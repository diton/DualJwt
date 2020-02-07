using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DualJwt.Security;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DualJwt
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<IApiKeyService, ApiKeyService>();

            // https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2836
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://localhost:5000";
                        options.RequireHttpsMetadata = false;
                        options.ApiName = "api1";
                        options.ApiSecret = "secret";
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseApiKeyAuthentication();

            //app.Use(async (ctx, next) =>
            //{
            //    if (ctx.User.Identity?.IsAuthenticated != true)
            //    {
            //        var result = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            //        if (result.Succeeded && result.Principal != null)
            //        {
            //            ctx.User = result.Principal;
            //        }
            //        else
            //        {
            //            var result2 = await ctx.AuthenticateAsync("IdentityBearer");
            //            if (result2.Succeeded && result2.Principal != null)
            //            {
            //                ctx.User = result2.Principal;
            //            }
            //        }
            //    }

            //    await next();
            //});

            app.UseMvc();
        }
    }
}

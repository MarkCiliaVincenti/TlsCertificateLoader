using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Runtime.Versioning;

namespace CertbotSample
{
    public class Startup
    {
        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = false;
                options.MaxAge = TimeSpan.FromDays(365 * 2);
            });

            services.AddSingleton<IWorker, Worker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHsts();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = context =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();

                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(30)
                    };
                }
            });

            app.Use(async (context, next) =>
              {
                  if (context.Request.IsHttps)
                  {
                      if (context.Request.Host.Host.StartsWith("www."))
                      {
                          var uriWww = context.Request.Path.Value ?? string.Empty;
                          var finalUri = $"https://{context.Request.Host.ToString()[4..]}{uriWww}";
                          context.Response.Redirect(finalUri, permanent: true);
                          return;
                      }
                      await next();
                      return;
                  }
                  var uri = context.Request.Path.Value ?? string.Empty;
                  if (uri.StartsWith("/.well-known"))
                  {
                      await next();
                      return;
                  }
                  var withHttps = $"https://{context.Request.Host}{uri}";
                  context.Response.Redirect(withHttps, permanent: true);
              });

            // Use Cookie Policy Middleware to conform to EU General Data 
            // Protection Regulation (GDPR) regulations.
            app.UseCookiePolicy();
        }
    }
}

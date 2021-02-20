using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

namespace AadProtectedApi {

    public class Startup {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(this.Configuration.GetSection("AzureAd"));

            services.AddControllers();

            services.AddSwaggerGen(c => {

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample Secure API", Version = "v1" });

                c.AddSecurityDefinition("aad-jwt", new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows {

                        Implicit = new OpenApiOAuthFlow {
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{this.Configuration.GetValue<string>("AzureAd:TenantId")}/oauth2/v2.0/authorize"),
                            Scopes = new Dictionary<string, string> {
                                { this.Configuration.GetValue<string>("AzureAd:Scope"), "Required Scope" }
                            }
                        },

                        AuthorizationCode = new OpenApiOAuthFlow {
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{this.Configuration.GetValue<string>("AzureAd:TenantId")}/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri($"https://login.microsoftonline.com/{this.Configuration.GetValue<string>("AzureAd:TenantId")}/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string> {
                                { this.Configuration.GetValue<string>("AzureAd:Scope"), "Required Scope" }
                            }
                        }
                    }

                }); ; ;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            if (env.IsDevelopment()) {

                app.UseDeveloperExceptionPage();

                IdentityModelEventSource.ShowPII = true;
            }

            // Allow Swagger, even in production
            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Secure API v1");
                c.OAuthClientId(this.Configuration.GetValue<string>("AzureAd:ClientId"));
                c.OAuthClientSecret(this.Configuration.GetValue<string>("AzureAd:ClientSecret"));
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
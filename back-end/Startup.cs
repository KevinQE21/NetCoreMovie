using back_end.Filtros;
using back_end.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace back_end
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
            //Crear filtro

            //Activar el cache
            services.AddResponseCaching();

            //Se agrega auth
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            //Tiempo mas corto de vida, cada vez que se pida la instancia, se obtiene una nueva instancia de memoria 
            services.AddTransient<IRepositorio, RepositorioEnMemoria>();
            //AddScoped el tiempo de vida va ser durante la peticion http, si varias clases dentro de la misma peticion a todos se les brindara la misma instancia
            //services.AddScoped<IRepositorio, RepositorioEnMemoria>();
            //AddSingleton el timpo de vida sera todo el tiempo de ejecucion de la aplicacion
            //services.AddSingleton<IRepositorio, RepositorioEnMemoria>();

            services.AddTransient<MiFiltroDeAccion>();

            //Se agrega un filtro de manera global a los controllers
            services.AddControllers(options => 
            {
                options.Filters.Add(typeof(FiltroExcepcion));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "back_end", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //Middleware para leer la respuesta y guardarla en un log
            app.Use(async (context, next) =>
            {
                using var swapStream = new MemoryStream();

                var respuestaOriginal = context.Response.Body;
                context.Response.Body = swapStream;

                await next.Invoke();

                swapStream.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(swapStream).ReadToEnd();
                swapStream.Seek(0, SeekOrigin.Begin);

                await swapStream.CopyToAsync(respuestaOriginal);
                context.Response.Body = respuestaOriginal;

                logger.LogInformation(respuesta);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "back_end v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Activa el caching
            app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

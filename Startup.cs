using System.Reflection;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using ApiVersioning.Swagger;

namespace ApiVersioning
{
#pragma warning disable 1591
    public class Startup
    {
        public IConfiguration _config { get; }
        private static string XmlCommentsFileName
        {
            get
            {
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return fileName;
            }
        }
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Swagger
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddControllers();

            services.AddSwaggerGen(
                       options =>
                       {
                           options.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);

                           // adicionar um filtro de operação personalizada que define os valores padrão
                           options.OperationFilter<SwaggerDefaultValues>();

                           // Inclui os comentários XML do assembly
                           // Defina o caminho de comentários para o JSON do Swagger e a UI.
                           options.IncludeXmlComments(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), XmlCommentsFileName));

                           options.UseAllOfForInheritance();

                           options.SelectSubTypesUsing(baseType =>
                           {
                               return typeof(Startup).Assembly.GetTypes().Where(type => type.IsSubclassOf(baseType));
                           });

                           options.CustomOperationIds(apiDesc =>
                           {
                               return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                           });

                           // options.SwaggerGeneratorOptions.IgnoreObsoleteActions = true;
                       });

            services.AddApiVersioning(options =>
            {
                // As versões da API de relatórios retornarão os cabeçalhos:
                // "api-supported-versions" (versões suportadas da api) 
                // "api-deprecated-versions" (versões descontinuadas da api) 
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(2, 0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                // obs: o código de formato especificado irá formatar a versão como "'v'principal[.segundária] [- status]"
                options.GroupNameFormat = "'v'VVV";
                // obs: esta opção só é necessária ao controlar a versão por segmento de url. O SubstitutionFormat
                // também pode ser usado para controlar o formato da versão da API em modelos de rota
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Constrói um endpoint Swagger para cada versão de API descoberta
                // e adiciona ao menu de navegação da UI.
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        url: $"/swagger/{description.GroupName}/swagger.json",
                        name: description.GroupName.ToUpperInvariant() + (description.IsDeprecated ? " - Is Deprecated" : string.Empty));
                }

                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.DisplayRequestDuration();
                options.DisplayOperationId();
                options.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
#pragma warning restore 1591
}

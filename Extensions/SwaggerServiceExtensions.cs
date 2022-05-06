using System.Reflection;
using ApiVersioning.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiVersioning.Extensions;

/// <summary>
///  SwaggerServiceExtensions class. This class cannot be inherited. 
/// </summary>
public static class SwaggerServiceExtensions
{
    private static string XmlCommentsFileName
    {
        get
        {
            var fileName = typeof(Program).GetTypeInfo().Assembly.GetName().Name + ".xml";
            return fileName;
        }
    }

    /// <summary>
    ///  Configura o Swagger para a API 
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
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
                           return typeof(Program).Assembly.GetTypes().Where(type => type.IsSubclassOf(baseType));
                       });

                       options.CustomOperationIds(apiDesc =>
                       {
                           return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                       });

                       options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                       {
                           Name = "Authorization",
                           Type = SecuritySchemeType.ApiKey,
                           Scheme = "Bearer",
                           BearerFormat = "JWT",
                           In = ParameterLocation.Header,
                           Description = "Cabeçalho de autorização JWT usando o esquema Bearer.\r\n\r\n Digite 'Bearer' [espaço] e, em seguida, seu token na entrada de texto abaixo.\r\n\r\nExemplo: \"Bearer 12345abcdef\"",
                       });

                       options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                new string[] {}
                            }
                        });
                   });

        return services;
    }

    /// <summary>
    ///  Adiciona o Swagger como serviço da aplicação.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
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

        return app;
    }
}

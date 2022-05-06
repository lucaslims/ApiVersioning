using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace ApiVersioning.Swagger;

/// <summary> 
/// Configura as opções de geração do Swagger.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    readonly IApiVersionDescriptionProvider provider;
    
    /// <summary>
    /// Inicializa uma nova instância da casse <see cref="ConfigureSwaggerOptions"/>.
    /// </summary>
    /// <param name="provider">O <see cref="IApiVersionDescriptionProvider">provedor</see> usado para gerar documentos Swagger.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // Adiciona um documento swagger para cada versão de API descoberta
        foreach (var description in provider.ApiVersionDescriptions)
        {
            try
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
            catch (Exception) { }
        }
    }

    static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Api",
            Version = description.ApiVersion.ToString()
        };

        switch (description.GroupName)
        {
            case "v1":
                info.Title = "Api";
                info.Description = "Web API em ASP.NET Core v6.0";
                info.Contact = new OpenApiContact
                {
                    Name = "Lucas Lima",
                    Email = "lucas@lucaslima.dev.br",
                    Url = new Uri("https://github.com/lucaslims"),
                };
                break;
            default:
                break;
        }

        if (description.IsDeprecated)
        {
            info.Description += " - This API version has been deprecated.";
        }
        return info;
    }
}

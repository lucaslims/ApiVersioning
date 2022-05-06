using Microsoft.AspNetCore.Mvc;

namespace ApiVersioning.Extensions;

public static class ApiVersioningServiceExtension
{
    public static IServiceCollection AddCustonApiVersioning(this IServiceCollection services)
    {

        services.AddApiVersioning(options =>
        {
            // As versões da API de relatórios retornarão os cabeçalhos:
            // "api-supported-versions" (versões suportadas da api) 
            // "api-deprecated-versions" (versões descontinuadas da api) 
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
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

        return services;
    }
}

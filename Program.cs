using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ApiVersioning.Extensions;
using ApiVersioning.Middleware;

var builder = WebApplication.CreateBuilder(args);
var _config = builder.Configuration;

// Add services to the container.
builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithOrigins("http://localhost", "https://localhost")
                    .WithHeaders("Accept", "Content-Type", "Origin", "X-Requested-With", "X-Custom-Header")
                    .WithMethods("GET", "POST")
                    );
            });

builder.Services.AddApplicationServices(_config);
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCustonApiVersioning();
builder.Services.AddIdentityService(_config);

var app = builder.Build();
var _provider = app.Services.GetService<IApiVersionDescriptionProvider>();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(_provider);
}

// app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

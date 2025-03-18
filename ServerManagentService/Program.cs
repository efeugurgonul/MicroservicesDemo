using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ServerManagentService.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ServerManagement API",
        Version = "v1",
        Description = "ServerManagement için API dokümantasyonu"
    });

    c.EnableAnnotations(); // Swagger annotation'larý etkinleþtir
});


// PostgreSQL baðlantýsýný ekleyin
builder.Services.AddDbContext<ServerManDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ServerManagementConnection")));

// MediatR'ý ekleyin
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

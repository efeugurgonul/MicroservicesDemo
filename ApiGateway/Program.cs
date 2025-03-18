using ApiGateway.Aggregators;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ocelot yap�land�rma dosyas�n� ekleme
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Servisleri ekleyin
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger'� yap�land�r�n
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway for Microservices"
    });

    // XML dok�mantasyonunu ekleyin (iste�e ba�l�)
    var xmlFile = $"ApiGateway.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Aggregator'� kaydet
builder.Services.AddSingleton<OrganizationWithProductsAggregator>();

// Ocelot servislerini ekleyin
builder.Services.AddOcelot(builder.Configuration)
    .AddSingletonDefinedAggregator<OrganizationWithProductsAggregator>();

// CORS politikas�n� ekleyin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Ocelot middleware'ini ekleyin
await app.UseOcelot();

app.MapControllers();

app.Run();
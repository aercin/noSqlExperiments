using Asp.Versioning;
using core_infrastructure.Extensions;
using core_infrastructure.Middlewares;
using infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Winton.Extensions.Configuration.Consul;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.  

builder.Configuration.AddConsul(
                        $"product/appsettings.{builder.Environment.EnvironmentName}.json",
                        options =>
                        {
                            options.ConsulConfigurationOptions = cco =>
                            {
                                cco.Address = new Uri(builder.Configuration.GetValue<string>("Consul:Address"));
                            };
                            options.ReloadOnChange = true;
                            options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                        });

builder.Configuration.AddVaultConfiguration(options =>
{
    options.MountPoint = builder.Configuration.GetValue<string>("Vault:MountPoint");
    options.Path = builder.Configuration.GetValue<string>("Vault:Path");
    options.Address = builder.Configuration.GetValue<string>("Vault:Address");
    options.AuthToken = builder.Configuration.GetValue<string>("VAULT_AUTH_TOKEN");
    options.ReloadOnChange = true;
    options.OnLoadException = (Exception ex) =>
    {
        var interimServiceProvider = builder.Services.BuildServiceProvider();
        var logger = interimServiceProvider.GetService<ILogger<Program>>();
        logger.LogError(exception: ex, message: $"Hata:{ex.Message}");
    };
});

builder.Services.AddControllers(); 

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("x-correlation-id");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName.Replace('+', '.'));
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Product Api", Version = "v1.0" });
    c.SwaggerDoc("v1.1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Product Api", Version = "v1.1" });
    c.DocumentFilter<PathLowercaseDocumentFilter>();
});

builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ReportApiVersions = true;
    //o.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("X-Version"));
    o.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
}).AddApiExplorer(//sadece swagger dokümantasyonu için gereklidir.
    options =>
    {
        options.GroupNameFormat = "'v'VVV"; //swagger dokümantasyonunda version combosunda versiyon bilgisi major.minor.patch format görünecek.
        options.SubstituteApiVersionInUrl = true;//swagger dokümantasyonunda version combosunda seçilen versiyon deðeri ep route bilgisine geçilecek.
    }
);


builder.Services.AddResponseCompression(x =>
{
    x.EnableForHttps = true;
});

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.AddInfrastructuralPipelines();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product v1.0 Api");
        c.SwaggerEndpoint("/swagger/v1.1/swagger.json", "Product v1.1 Api");
    });
}

app.UseResponseCompression();

app.UseMiddleware<CorrelationMiddleware>();

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();


public class PathLowercaseDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var dictionaryPath = swaggerDoc.Paths.ToDictionary(x => ToLowercase(x.Key), x => x.Value);
        var newPaths = new OpenApiPaths();
        foreach (var path in dictionaryPath)
        {
            newPaths.Add(path.Key, path.Value);
        }
        swaggerDoc.Paths = newPaths;
    }

    private static string ToLowercase(string key)
    {
        var parts = key.Split('/').Select(part => part.Contains("}") ? part : part.ToLowerInvariant());
        return string.Join('/', parts);
    }
}


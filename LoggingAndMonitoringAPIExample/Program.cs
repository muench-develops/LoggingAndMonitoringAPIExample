using AutoMapper;
using LoggingAndMonitoringAPIExample.Handler;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging((hostingContext, loggingBuilder) =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
        | ActivityTrackingOptions.TraceId
        | ActivityTrackingOptions.ParentId;
    });
    //loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
    loggingBuilder.AddEventSourceLogger();
});


// Add services to the container.
builder.Services.AddControllers(cfg =>
{
    cfg.ReturnHttpNotAcceptable = true;
})
.AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api", Version = "v1" });
});

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
builder.Services.AddScoped<IMemoryCache, MemoryCache>();
// Add Automapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new CustomerMappingProfile());
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<CustomerControllerDependencyHandler>();


// Add in Memory Db
builder.Services.AddDbContext<CustomerDbContext>(options => options.UseInMemoryDatabase("CustomerInMemoryDb"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected fault happend. Try again later.");
        });
    });
}

app.UseResponseCaching();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
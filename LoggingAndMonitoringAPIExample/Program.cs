using AutoMapper;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using LoggingAndMonitoringAPIExample.Handler;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var tracePath = Path.Join(path, $"Log_CustomerService_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
Trace.Listeners.Add(new TextWriterTraceListener(tracePath));
Trace.AutoFlush = true;

builder.Services.AddProblemDetails(opts =>
{
    opts.IncludeExceptionDetails = (ctx, ex) => false;
    opts.OnBeforeWriteDetails = (ctx, dtls) =>
    {
        if (dtls.Status == 500)
            dtls.Detail = "An error occured. Use Trace Id when contacting us.";
    };
});

builder.Host.ConfigureLogging((hostingContext, loggingBuilder) =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
        | ActivityTrackingOptions.TraceId
        | ActivityTrackingOptions.ParentId;
    });
    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
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
builder.Services.AddMemoryCache();

// Add Automapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new CustomerMappingProfile());
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<CustomerDependencyHandler>();


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
    app.UseProblemDetails();
}

app.UseResponseCaching();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
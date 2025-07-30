using Disclosure.Shared.Data;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ApplicationDbContext>("disclosure");
//builder.AddNpgsqlDataSource(connectionName: "postgres");

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Add healthcheck endpoint. 
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// get IServiceProvider for our Endpoint mapping, used for getting DB context etc. 
IServiceProvider serviceProvider = app.Services;

// Map all endpoints in the Disclosure.Services.Api.Endpoints namespace
foreach (Type endpointType in AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(assembly => assembly.GetTypes())
    .Where(type => type.IsClass && !type.IsAbstract && type.Namespace?.StartsWith("Disclosure.Services.Api.Endpoints") == true))
{
    if (endpointType?.DeclaringType?.Name == "Endpoint")
    {
        MethodInfo? method = endpointType.DeclaringType.GetMethod("AddEndpoint", BindingFlags.Static | BindingFlags.Public);
        if (method == null)
        {
            // If the method is not found, throw an exception
            throw new InvalidOperationException($"{endpointType.DeclaringType.FullName} must implement RequiredMethod.");
        }
        else
        {
            method.Invoke(null, [app, serviceProvider]);
        }
    }
}

app.Run();

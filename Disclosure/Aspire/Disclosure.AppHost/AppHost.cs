var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("db-username", secret: true);
var password = builder.AddParameter("db-password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password)
                      .WithDataVolume(isReadOnly: false);

var postgresdb = postgres.AddDatabase("disclosure");

var cache = builder.AddRedis("cache");

var apiService =
builder.AddProject<Projects.Disclosure_Services_Api>("services-api")
    .WithReference(postgresdb)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Disclosure_Web>("clients-web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

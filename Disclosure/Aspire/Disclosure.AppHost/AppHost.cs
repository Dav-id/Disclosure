IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> username = builder.AddParameter("db-username", secret: true);
IResourceBuilder<ParameterResource> password = builder.AddParameter("db-password", secret: true);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres", username, password)
                                                           .WithDataVolume(isReadOnly: false);

IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres.AddDatabase("disclosure");

IResourceBuilder<RedisResource> cache = builder.AddRedis("cache");

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.Disclosure_Services_Api>("services-api")
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

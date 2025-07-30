using Disclosure.Shared.Data;

using static Disclosure.Services.Api.Endpoints.Account.GetProfile.Contracts;

namespace Disclosure.Services.Api.Endpoints.Account.GetProfile
{
    public static class Endpoint
    {
        public static void AddEndpoint(this IEndpointRouteBuilder app, IServiceProvider services)
        {
            app.MapPost("api/v1/users", (Request request) =>
            {
                var response = GetProfile(request, services);

                return Results.Ok(response);
            });
        }

        private static Response GetProfile(Request request, IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            

            //Collections.Users.Add(request);

            return new Response(request.Id, "First Name",  "Last Name", true);
        }

    }
}

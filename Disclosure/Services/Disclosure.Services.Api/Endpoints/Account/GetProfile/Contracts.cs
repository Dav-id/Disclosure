namespace Disclosure.Services.Api.Endpoints.Account.GetProfile
{
    public class Contracts
    {
        public record Response(string Id,
            string FirstName,
                          string LastName,
                          bool IsEnabled);

        public record Request(string Id);
    }
}

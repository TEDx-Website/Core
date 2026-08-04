using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Infrastructure.Configuration;

namespace TEDx.Infrastructure.Identity;

internal sealed class AuthLinkBuilder : IAuthLinkBuilder
{
    private const string EmailParameter = "email";
    private const string TokenParameter = "token";

    private readonly IOptions<FrontendOptions> _options;

    public AuthLinkBuilder(IOptions<FrontendOptions> options)
    {
        _options = options;
    }

    public string BuildPasswordReset(string email, string token)
        => Build(_options.Value.ResetPasswordPath, email, token);

    public string BuildEmailConfirmation(string email, string token)
        => Build(_options.Value.ConfirmEmailPath, email, token);

    private string Build(string path, string email, string token)
    {
        var page = new Uri(
            new Uri(_options.Value.BaseUrl.TrimEnd('/') + "/"),
            path.TrimStart('/'));

        return QueryHelpers.AddQueryString(
            page.ToString(),
            new Dictionary<string, string?>
            {
                [EmailParameter] = email,
                [TokenParameter] = token,
            });
    }
}

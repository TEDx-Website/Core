using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Infrastructure.Configuration;

namespace TEDx.Infrastructure.Identity;

internal sealed class PasswordResetLinkBuilder : IPasswordResetLinkBuilder
{
    private const string EmailParameter = "email";
    private const string TokenParameter = "token";

    private readonly IOptions<FrontendOptions> _options;

    public PasswordResetLinkBuilder(IOptions<FrontendOptions> options)
    {
        _options = options;
    }

    public string Build(string email, string token)
    {
        var options = _options.Value;

        var page = new Uri(
            new Uri(options.BaseUrl.TrimEnd('/') + "/"),
            options.ResetPasswordPath.TrimStart('/'));

        return QueryHelpers.AddQueryString(
            page.ToString(),
            new Dictionary<string, string?>
            {
                [EmailParameter] = email,
                [TokenParameter] = token,
            });
    }
}

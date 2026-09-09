using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Infrastructure.Options;

namespace TEDx.Infrastructure.Identity;

internal sealed class AuthLinkBuilder : IAuthLinkBuilder
{
    private const string EmailParameter = "email";
    private const string UserIdParameter = "userId";
    private const string TokenParameter = "token";

    private readonly IOptions<FrontendOptions> _options;

    public AuthLinkBuilder(IOptions<FrontendOptions> options)
    {
        _options = options;
    }

    public string BuildPasswordReset(string email, string token)
    {
        var page = BuildPageUri(_options.Value.ResetPasswordPath);

        return QueryHelpers.AddQueryString(
            page,
            new Dictionary<string, string?>
            {
                [EmailParameter] = email,
                [TokenParameter] = token,
            });
    }

    public string BuildEmailConfirmation(Guid userId, string token)
    {
        var page = BuildPageUri(_options.Value.ConfirmEmailPath);

        return QueryHelpers.AddQueryString(
            page,
            new Dictionary<string, string?>
            {
                [UserIdParameter] = userId.ToString(),
                [TokenParameter] = token,
            });
    }

    private string BuildPageUri(string path)
    {
        var page = new Uri(
            new Uri(_options.Value.BaseUrl.TrimEnd('/') + "/"),
            path.TrimStart('/'));

        return page.ToString();
    }
}

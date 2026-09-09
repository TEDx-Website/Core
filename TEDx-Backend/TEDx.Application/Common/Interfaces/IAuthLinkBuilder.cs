namespace TEDx.Application.Common.Interfaces;

public interface IAuthLinkBuilder
{
    string BuildPasswordReset(string email, string token);

    string BuildEmailConfirmation(Guid userId, string token);
}

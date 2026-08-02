using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Identity.DTOs.Login;
using TEDx.Application.Identity.DTOs.Register;
using TEDx.Domain.Common;
namespace TEDx.Application.Identity.Command.Login
{
    public sealed class LoginCommand : IRequest<Result<AuthResponse>>
    {
        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }
        public string Password { get; }
    }
}

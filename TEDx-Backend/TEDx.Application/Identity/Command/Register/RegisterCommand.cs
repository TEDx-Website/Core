using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;
using TEDx.Application.Identity.DTOs.Register;

namespace TEDx.Application.Identity.Command.Register
{
    public sealed class RegisterCommand : IRequest<Result<RegisterResponse>>
    {
        public RegisterCommand(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Password { get; }
        public string ConfirmPassword { get; }

    }
    
}

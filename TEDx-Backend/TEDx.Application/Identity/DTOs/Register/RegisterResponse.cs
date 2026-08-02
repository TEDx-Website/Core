using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Application.Identity.DTOs.Register
{
    public sealed class RegisterResponse
    {
        public RegisterResponse(Guid id, string email, string firstName, string lastName, GlobalRole globalRole, bool emailConfirmationRequired)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            GlobalRole = globalRole;
            EmailConfirmationRequired = emailConfirmationRequired;
        }
        public Guid Id { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public GlobalRole GlobalRole { get; }
        public bool EmailConfirmationRequired { get; }  
    }

}

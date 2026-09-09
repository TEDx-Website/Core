using TEDx.Domain.Communication.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Communication.Entities
{
    public class ContactMessage : AuditableEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Subject { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public ContactStatus Status { get; private set; }

        public static ContactMessage Create(
            string name,
            string email,
            string subject,
            string message)
        {
            return new ContactMessage
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                Subject = subject,
                Message = message,
                Status = ContactStatus.New
            };
        }

        // guardless
        public void ChangeStatus(ContactStatus status)
        {
            Status = status;
        }
    }
}

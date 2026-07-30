using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;

namespace TEDx.Domain.Communication
{
    public class ContactMessage : IContactAudit
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Subject { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public ContactStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}

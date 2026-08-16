using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Communication.Entities
{
    public class NotificationRecipient
    {
        public Guid Id { get; private set; }

        public Guid NotificationId { get; set; }

        public Guid AccountId { get; set; }

        public bool IsRead { get;  set; }

        public DateTime? ReadAtUtc { get; set; }

        public DateTime? CreatedAtUtc { get;  set; }
        public Notification Notification { get;  set; } = null!;
    }
}

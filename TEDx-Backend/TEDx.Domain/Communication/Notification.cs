using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Communication
{
    public class Notification 
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; } = null!;
        public string Body { get; private set; } = null!;

        public NotificationAudienceType AudienceType { get;  set; }

        public Guid? AudienceRoleId { get; private set; }

        public Guid? TrackId { get; private set; }

        public Guid SentBy { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
        public List<NotificationRecipient>? NotificationRecipients { get; set; }
    }
}

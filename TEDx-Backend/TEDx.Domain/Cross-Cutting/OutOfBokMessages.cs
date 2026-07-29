using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Cross_Cutting
{
    public class OutOfBokMessages
    {
        public Guid Id { get; private set; }

        public string Type { get; private set; } = null!;

        public string PayloadJson { get; private set; } = null!;

        public DateTime? ProcessedAtUtc { get; private set; }

        public int Attempts { get; private set; }

        public string? LastError { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
    }
}

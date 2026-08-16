using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.Abstractions
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAtUtc { get; set; }
    }
}
/*
 Entity                Audit    SoftDelete    RowVersion

User                  ✔            ✘             ✔
Event                 ✔            ✘             ✔
Ticket                ✔            ✘             ✔
Category              ✔            ✔             ✘
Product               ✔            ✔             ✔
TrainingSession       ✔            ✘             ✔

 */

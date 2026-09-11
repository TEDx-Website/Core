using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Speaker
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public string TagLine { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsTopSpeaker { get; set; }
        public List<Event> Events { get; set; } = new();

    }
}

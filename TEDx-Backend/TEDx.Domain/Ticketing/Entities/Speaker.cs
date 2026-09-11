using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Speaker
    {
        public Guid Id { get; set; }

        // Speaker Info
        public string SpeakerName { get; set; } = null!;
        public string SpeakerPictureUrl { get; set; } = null!;
        public string SpeakerRole { get; set; } = null!; // Professional Role / Title & Affiliation
        public string SpeakerBio { get; set; } = null!; // Full Biography

        // Talk Info
        public string TalkTitle { get; set; } = null!;
        public string TalkTrack { get; set; } = null!; // The track the talk belongs to
        public string TalkShortDescription { get; set; } = null!; // Brief summary of the talk (shown on card hover)

        // Spotlight & Order
        public bool IsNextSpeaker { get; set; }
        public bool IsTopSpeaker { get; set; }
        public int? TopSpeakerOrderIndex { get; set; }

        // Speaker Social Links
        public string? SpeakerLinkedInUrl { get; set; }
        public string? SpeakerXUrl { get; set; }
        public string? SpeakerWebsiteUrl { get; set; }

        public List<Event> Events { get; set; } = new();
    }
}

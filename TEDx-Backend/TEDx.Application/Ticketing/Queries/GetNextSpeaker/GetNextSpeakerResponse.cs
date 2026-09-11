namespace TEDx.Application.Ticketing.Queries.GetNextSpeaker
{
    public sealed record NextSpeakerResponse(
        Guid Id,
        string TalkTrack,       // Badge / Track (e.g. "AI & Neuro-Cognitive Systems · Keynote")
        string SpeakerName,
        string SpeakerRole,
        string TalkTitle,
        string SpeakerBio,      // Bio / Talk Summary shown in the Hero
        string SpeakerPictureUrl);
}

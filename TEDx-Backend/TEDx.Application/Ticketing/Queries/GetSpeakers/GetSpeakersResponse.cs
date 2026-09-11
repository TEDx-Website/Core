namespace TEDx.Application.Ticketing.Queries.GetSpeakers
{
    public sealed record SpeakerCatalogResponse(
        int TotalCount,
        List<SpeakerCatalogItem> Speakers);

    public sealed record SpeakerCatalogItem(
        Guid Id,
        string SpeakerName,
        string SpeakerPictureUrl,
        string SpeakerRole,
        string TalkTrack,
        string TalkTitle,
        string TalkShortDescription,
        string SpeakerBio,
        SocialLinksResponse SocialLinks);

    public sealed record SocialLinksResponse(
        string? LinkedInUrl,
        string? XUrl,
        string? WebsiteUrl);
}

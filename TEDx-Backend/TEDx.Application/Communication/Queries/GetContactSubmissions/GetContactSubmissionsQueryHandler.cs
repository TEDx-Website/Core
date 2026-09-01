using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Communication.Dtos;
using TEDx.Domain.Common;
using TEDx.Domain.Communication.Entities;
using TEDx.Domain.Communication.Enums;

namespace TEDx.Application.Communication.Queries.GetContactSubmissions;

public sealed class GetContactSubmissionsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetContactSubmissionsQuery, Result<PagedResult<ContactSubmissionListItemDto>>>
{
    public const int ExcerptLength = 120;

    private const string StatusParameterName = "status";
    private const int MaxEchoedLength = 40;

    public async Task<Result<PagedResult<ContactSubmissionListItemDto>>> Handle(
        GetContactSubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var status = ParseStatus(request.Status);
        if (status.IsError)
        {
            return Result<PagedResult<ContactSubmissionListItemDto>>.Failure(status.Errors);
        }

        var filtered = ApplyStatusFilter(db.ContactMessages.AsNoTracking(), status.Value);

        var ordered = ContactSorting.Admin.Apply(filtered, request.Sort);
        if (ordered.IsError)
        {
            return Result<PagedResult<ContactSubmissionListItemDto>>.Failure(ordered.Errors);
        }

        var projected = ordered.Value.Select(m => new ContactSubmissionListItemDto(
            m.Id,
            m.Name,
            m.Email,
            m.Subject,
            m.Message.Substring(0, ExcerptLength),
            m.Status,
            m.CreatedAtUtc));

        var page = await projected.ToPagedResultAsync(
            PagedRequest.From(request.Page, request.PageSize),
            cancellationToken);

        return Result<PagedResult<ContactSubmissionListItemDto>>.Success(page);
    }

    private static Result<ContactStatus?> ParseStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ContactStatus?>.Success(null);
        }

        var token = value.Trim();
        var names = Enum.GetNames<ContactStatus>();
        var name = Array.Find(
            names,
            allowed => string.Equals(allowed, token, StringComparison.OrdinalIgnoreCase));

        if (name is not null)
        {
            return Result<ContactStatus?>.Success(Enum.Parse<ContactStatus>(name));
        }

        return Error.Validation(
            CommonErrors.ValidationError.Code,
            $"Value '{(token.Length <= MaxEchoedLength ? token : token[..MaxEchoedLength] + "…")}' "
            + $"is not supported for '{StatusParameterName}'. "
            + $"Allowed values: {string.Join(", ", names)}.",
            StatusParameterName);
    }

    private static IQueryable<ContactMessage> ApplyStatusFilter(
        IQueryable<ContactMessage> source,
        ContactStatus? status)
    {
        if (status is null)
        {
            return source;
        }

        var value = status.Value;
        return source.Where(m => m.Status == value);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Pagination;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Application.Ticketing.Queries.GetAdminSpeakers
{
    public sealed class GetAdminSpeakersQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<
        GetAdminSpeakersQuery,
        Result<PagedResult<AdminSpeakerResponse>>>
    {
        public async Task<Result<PagedResult<AdminSpeakerResponse>>> Handle(
            GetAdminSpeakersQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.Speakers.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var page = PagedRequest.From(
                request.Page,
                request.PageSize);

            var speakers = await query
                .OrderBy(s => s.Name)
                .Skip(page.Skip)
                .Take(page.Take)
                .Select(s => new AdminSpeakerResponse(
                    s.Id,
                    s.Name,
                    s.PictureUrl,
                    s.TagLine,
                    s.Description,
                    s.Events
                        .Select(e => e.TitleEn)
                        .ToList()
                ))
                .ToListAsync(cancellationToken);

            return Result<PagedResult<AdminSpeakerResponse>>.Success(
                PagedResult<AdminSpeakerResponse>.Create(
                    speakers,
                    page,
                    totalCount));
        }
    }
}

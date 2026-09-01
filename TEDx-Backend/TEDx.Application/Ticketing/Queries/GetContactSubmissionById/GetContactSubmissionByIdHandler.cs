using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Communication.Entities;
using TEDx.Application.Common.Errors;
using TEDx.Domain.Common;


namespace TEDx.Application.Ticketing.Queries.GetContactSubmissionById
{
    internal class GetContactSubmissionByIdHandler : IRequestHandler<GetContactSubmissionByIdQuery, Result<GetContactSubmissionByIdResponse>>
    {
        private readonly IApplicationDbContext context;
        public GetContactSubmissionByIdHandler(IApplicationDbContext dbContext)
        {
            context = dbContext;
        }
        public async Task<Result<GetContactSubmissionByIdResponse>> Handle(
            GetContactSubmissionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var submission = await context.ContactMessages
                                 .Where(x => x.Id == request.Id)
                                 .Select(x => new GetContactSubmissionByIdResponse
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Email = x.Email,
                                     Subject = x.Subject,
                                     Message = x.Message,
                                     Status = x.Status,
                                     CreatedAtUtc = x.CreatedAtUtc,
                                     UpdatedAtUtc = x.UpdatedAtUtc,
                                     UpdatedBy = x.UpdatedBy
                                 })
                                 .FirstOrDefaultAsync(cancellationToken);

            if (submission is null)
            {
                return Result<GetContactSubmissionByIdResponse>.Failure(CommonErrors.NotFound);
            }

            return submission;
        }
    }
}

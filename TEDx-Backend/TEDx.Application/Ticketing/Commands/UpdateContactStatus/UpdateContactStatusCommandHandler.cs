using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Application.Common.Errors;
namespace TEDx.Application.Ticketing.Commands.UpdateContactStatus
{
    public class UpdateContactStatusCommandHandler : IRequestHandler<UpdateContactStatusCommand, Result<UpdateContactStatusResponse>>
    {
        private readonly IApplicationDbContext dbContext;
        public UpdateContactStatusCommandHandler(IApplicationDbContext context, IClock _clock)
        {
            dbContext = context;
        }
        public async Task<Result<UpdateContactStatusResponse>> Handle(UpdateContactStatusCommand request, CancellationToken cancellationToken)
        {
            var message = await dbContext.ContactMessages.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if(message is null)
            {
                return Result<UpdateContactStatusResponse>.Failure(CommonErrors.NotFound);
            }

            message.ChangeStatus(request.Status);
            await dbContext.SaveChangesAsync(cancellationToken);
            var response = new UpdateContactStatusResponse(message.Id, message.Status);

            return Result<UpdateContactStatusResponse>.Success(response);
        }
    }
}

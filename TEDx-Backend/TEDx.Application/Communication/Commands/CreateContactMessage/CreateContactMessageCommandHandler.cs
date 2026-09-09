using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Communication.Entities;

namespace TEDx.Application.Communication.Commands.CreateContactMessage
{
    public sealed class CreateContactMessageCommandHandler
        : IRequestHandler<CreateContactMessageCommand, Result<CreateContactMessageResponse>>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateContactMessageCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<CreateContactMessageResponse>> Handle(
            CreateContactMessageCommand request,
            CancellationToken cancellationToken)
        {
            var contactMessage = ContactMessage.Create(
                name: request.Name,
                email: request.Email,
                subject: request.Subject,
                message: request.Message);

            await _dbContext.ContactMessages.AddAsync(contactMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new CreateContactMessageResponse(
                contactMessage.Id,
                contactMessage.Status);

            return Result<CreateContactMessageResponse>.Success(response);
        }
    }
}

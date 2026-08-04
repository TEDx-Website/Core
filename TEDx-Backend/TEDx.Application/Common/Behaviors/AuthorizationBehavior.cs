using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;
using TEDx.Domain.Training.Enums;

namespace TEDx.Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ICurrentUser _currentUser;
        private readonly ITrackAccessReader _trackAccess;
        private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

        public AuthorizationBehavior(
            ICurrentUser currentUser,
            ITrackAccessReader trackAccess,
            ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
        {
            _currentUser = currentUser;
            _trackAccess = trackAccess;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            // مفيش marker = مفتوح
            if (request is not IRequireAuthentication)
                return await next();

            var userId = _currentUser.UserId;

            if (!_currentUser.IsAuthenticated || userId is null)
                return Failure(Errors_Identity.Unauthenticated);

            if (request is IRequireAdmin && !_currentUser.IsAdmin)
                return Failure(Errors_Identity.Forbidden);

            if (request is ITrackScopedRequest scoped)
            {
                if (!_currentUser.IsAdmin)
                {
                    var role = await _trackAccess
                        .GetRoleInTrackAsync(userId.Value, scoped.TrackId, ct);

                    var allowed = request switch
                    {
                        IRequireBoardOfTrack => role == TrackRole.Board,
                        IRequireMemberOfTrack => role == TrackRole.Member,
                        _ => role is not null
                    };

                    if (!allowed)
                    {
                        _logger.LogWarning(
                            "Track access denied. Account {AccountId}, Track {TrackId}, " +
                            "ActualRole {Role}, Request {Request}",
                            userId, scoped.TrackId, role, typeof(TRequest).Name);

                        return Failure(Errors_Training.TrackForbidden);
                    }
                }
            }

            return await next();
        }

        private static TResponse Failure(Error error) =>
            FailureResponseFactory.Create<TResponse>(error);
    }
}

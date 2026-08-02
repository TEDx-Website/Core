using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Training.Enums;
using TEDx.Domain.Common;
using TEDx.Application.Common.Errors;
using System.Reflection;

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
            ResultFactory.FailureOf<TResponse>(error);
    }

    public static class ResultFactory
    {
        public static TResponse FailureOf<TResponse>(Error error)
        {
            var type = typeof(TResponse);

            // Result بسيط
            if (type == typeof(Error))
                return (TResponse)(object)error;

            // Result<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = type.GetGenericArguments()[0];

                var method = typeof(Error)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m => m.Name == nameof(Error.Failure)
                             && m.IsGenericMethod
                             && m.GetParameters().Length == 1)
                    .MakeGenericMethod(valueType);

                return (TResponse)method.Invoke(null, new object[] { error })!;
            }

            throw new InvalidOperationException(
                $"AuthorizationBehavior cannot build a failure for {type.Name}. " +
                $"All authorized requests must return Result or Result<T>.");
        }
    }
}

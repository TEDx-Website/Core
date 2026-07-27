using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Errors;

namespace TEDx.Application.Common.Errors
{
    public static class Errors
    {
        public static readonly Error ValidationError =
            new("VALIDATION_ERROR", "Validation failed.", ErrorType.Validation);

        public static readonly Error Unauthenticated =
            new("UNAUTHENTICATED", "Authentication is required.", ErrorType.Unauthorized);

        public static readonly Error Forbidden =
            new("FORBIDDEN", "You are not allowed to perform this action.", ErrorType.Unauthorized);

        public static readonly Error TrackForbidden =
            new("TRACK_FORBIDDEN", "You are not allowed to access this track.", ErrorType.Unauthorized);

        public static readonly Error NotFound =
            new("NOT_FOUND", "The requested resource was not found.", ErrorType.NotFound);

        public static readonly Error ConcurrencyConflict =
            new("CONCURRENCY_CONFLICT", "The resource was modified by another operation.", ErrorType.Conflict);

        public static readonly Error RateLimited =
            new("RATE_LIMITED", "Too many requests.", ErrorType.Business);

        public static readonly Error EmailTaken =
            new("EMAIL_TAKEN", "Email is already in use.", ErrorType.Conflict);

        public static readonly Error InvalidCredentials =
            new("INVALID_CREDENTIALS", "Invalid email or password.", ErrorType.Unauthorized);

        public static readonly Error CurrentPasswordIncorrect =
            new("CURRENT_PASSWORD_INCORRECT", "Current password is incorrect.", ErrorType.Unauthorized);

        public static readonly Error AccountDeactivated =
            new("ACCOUNT_DEACTIVATED", "Account has been deactivated.", ErrorType.Unauthorized);

        public static readonly Error WeakPassword =
            new("WEAK_PASSWORD", "Password does not meet security requirements.", ErrorType.Validation);

        public static readonly Error TokenInvalid =
            new("TOKEN_INVALID", "Token is invalid.", ErrorType.Unauthorized);

        public static readonly Error TokenReused =
            new("TOKEN_REUSED", "Token has already been used.", ErrorType.Unauthorized);

        public static readonly Error ResetTokenInvalid =
            new("RESET_TOKEN_INVALID", "Reset token is invalid.", ErrorType.Unauthorized);

        public static readonly Error TicketAlreadyCheckedIn =
            new("TICKET_ALREADY_CHECKED_IN", "Ticket has already been checked in.", ErrorType.Business);

        public static readonly Error WrongEvent =
            new("WRONG_EVENT", "Ticket does not belong to this event.", ErrorType.Business);

        public static readonly Error TicketVoided =
            new("TICKET_VOIDED", "Ticket has been voided.", ErrorType.Business);

        public static readonly Error TicketInvalid =
            new("TICKET_INVALID", "Ticket is invalid.", ErrorType.Business);

        public static readonly Error PriceChanged =
            new("PRICE_CHANGED", "The ticket price has changed.", ErrorType.Conflict);

        public static readonly Error SeatsUnavailable =
            new("SEATS_UNAVAILABLE", "Requested seats are unavailable.", ErrorType.Conflict);

        public static readonly Error QuantityExceedsMax =
            new("QUANTITY_EXCEEDS_MAX", "Requested quantity exceeds the allowed maximum.", ErrorType.Business);

        public static readonly Error ActiveOrderExists =
            new("ACTIVE_ORDER_EXISTS", "An active order already exists.", ErrorType.Conflict);

        public static readonly Error HoldExpired =
            new("HOLD_EXPIRED", "The seat hold has expired.", ErrorType.Conflict);

        public static readonly Error OrderNotCancellable =
            new("ORDER_NOT_CANCELLABLE", "Order cannot be cancelled.", ErrorType.Business);

        public static readonly Error OrderNotPayable =
            new("ORDER_NOT_PAYABLE", "Order cannot be paid.", ErrorType.Business);

        public static readonly Error OrderIsFree =
            new("ORDER_IS_FREE", "Order is already free.", ErrorType.Business);

        public static readonly Error OrderNotFree =
            new("ORDER_NOT_FREE", "Order is not free.", ErrorType.Business);

        public static readonly Error PromoCodeTaken =
            new("PROMO_CODE_TAKEN", "Promo code has already been used.", ErrorType.Conflict);

        public static readonly Error PromoInactive =
            new("PROMO_INACTIVE", "Promo code is inactive.", ErrorType.Business);

        public static readonly Error PromoNotYetValid =
            new("PROMO_NOT_YET_VALID", "Promo code is not yet valid.", ErrorType.Business);

        public static readonly Error PromoExpired =
            new("PROMO_EXPIRED", "Promo code has expired.", ErrorType.Business);

        public static readonly Error PromoCapReached =
            new("PROMO_CAP_REACHED", "Promo code usage limit has been reached.", ErrorType.Business);

        public static readonly Error PromoUserLimit =
            new("PROMO_USER_LIMIT", "User has reached the promo code usage limit.", ErrorType.Business);

        public static readonly Error PromoWrongEvent =
            new("PROMO_WRONG_EVENT", "Promo code is not valid for this event.", ErrorType.Business);

        public static readonly Error NoRecipientsResolved =
            new("NO_RECIPIENTS_RESOLVED", "No recipients could be resolved.", ErrorType.Business);

        public static readonly Error IllegalStatusTransition =
           new("ILLEGAL_STATUS_TRANSITION", "the status is illegal.", ErrorType.Conflict);

        public static readonly Error EventHasOrders =
          new("EVENT_HAS_ORDERS", "Event has order.", ErrorType.Conflict);

        public static readonly Error SessionHasRecords =
          new("SESSION_HAS_RECORDS", "Session has records.", ErrorType.Conflict);
        public static readonly Error CapacityBelowSolid =
          new("CAPACITY_BELOW_SOLD", "Capacity is Solid.", ErrorType.Conflict);

    }
}

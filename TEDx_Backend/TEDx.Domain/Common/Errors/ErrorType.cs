using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.Errors
{ 
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Business,
        Unauthorized
        //VALIDATION_ERROR,
        //UNAUTHENTICATED,
        //FORBIDDEN,
        //TRACK_FORBIDDEN,
        //NOT_FOUND,
        //CONCURRENCY_CONFLICT,
        //RATE_LIMITED,
        //EMAIL_TAKEN,
        //INVALID_CREDENTIALS,
        //CURRENT_PASSWORD_INCORRECT,
        //ACCOUNT_DEACTIVATED, 
        //WEAK_PASSWORD,
        //TOKEN_INVALID,
        //TOKEN_REUSED,
        //RESET_TOKEN_INVALID,
        //TICKET_ALREADY_CHECKED_IN, 
        //WRONG_EVENT,
        //TICKET_VOIDED,
        //TICKET_INVALID, 
        //PRICE_CHANGED,
        //SEATS_UNAVAILABLE,
        //QUANTITY_EXCEEDS_MAX,
        //ACTIVE_ORDER_EXISTS,
        //HOLD_EXPIRED,
        //ORDER_NOT_CANCELLABLE,
        //ORDER_NOT_PAYABLE,
        //ORDER_IS_FREE,
        //ORDER_NOT_FREE,
        //PROMO_CODE_TAKEN,
        //PROMO_INACTIVE,
        //PROMO_NOT_YET_VALID,
        //PROMO_EXPIRED,
        //PROMO_CAP_REACHED,
        //PROMO_USER_LIMIT,
        //PROMO_WRONG_EVENT,
        //NO_RECIPIENTS_RESOLVED
    }
}

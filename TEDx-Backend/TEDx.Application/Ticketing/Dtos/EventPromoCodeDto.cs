using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Dtos;

public sealed record EventPromoCodeDto(
    Guid Id,
    string? Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    int RedemptionCount,
    int GlobalRedemptionCap,
    int PerUserLimit,
    bool IsActive,
    DateTime ValidFromUtc,
    DateTime ValidUntilUtc);

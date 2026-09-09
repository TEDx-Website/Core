namespace TEDx.Application.Common.Dtos
{
    public sealed record PackageResponse(
    Guid Id,
    string NameEn,
    string NameAr,
    int SeatsPerPackage,
    int? MaxQuantityPerOrder,
    MoneyDto Price,
    bool IsActive
);
}

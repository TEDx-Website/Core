using System.Linq.Expressions;
using TEDx.Application.Common.Errors;
using TEDx.Domain.Common;

namespace TEDx.Application.Common.Pagination;

public sealed class SortWhitelist<T>
{
    private delegate IOrderedQueryable<T> Ordering(IQueryable<T> source, SortDirection direction);

    private readonly Dictionary<string, Ordering> _orderings = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _allowedFields = [];

    private Expression<Func<T, Guid>>? _tieBreaker;
    private string? _defaultField;
    private SortDirection _defaultDirection;

    public IReadOnlyList<string> AllowedFields => _allowedFields;

    public SortWhitelist<T> Allow<TKey>(string field, Expression<Func<T, TKey>> keySelector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentNullException.ThrowIfNull(keySelector);

        var added = _orderings.TryAdd(field, (source, direction) => direction == SortDirection.Descending
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector));

        if (!added)
        {
            throw new InvalidOperationException($"Sort field '{field}' is already whitelisted.");
        }

        _allowedFields.Add(field);
        return this;
    }

    public SortWhitelist<T> TieBreakBy(Expression<Func<T, Guid>> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);

        _tieBreaker = keySelector;
        return this;
    }

    public SortWhitelist<T> WithDefault(string field, SortDirection direction = SortDirection.Ascending)
    {
        if (!_orderings.ContainsKey(field))
        {
            throw new InvalidOperationException(
                $"Default sort field '{field}' is not whitelisted. "
                + $"Whitelisted: {string.Join(", ", _allowedFields)}.");
        }

        _defaultField = field;
        _defaultDirection = direction;
        return this;
    }

    public Result<IOrderedQueryable<T>> Apply(IQueryable<T> source, string? sort)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (_defaultField is null)
        {
            throw new InvalidOperationException(
                $"This whitelist has no default sort. Call {nameof(WithDefault)} when declaring it.");
        }

        var field = _defaultField;
        var direction = _defaultDirection;

        if (!string.IsNullOrWhiteSpace(sort))
        {
            var parts = sort.Split(SortSyntax.Separator);
            if (parts.Length > 2)
            {
                return CommonErrors.MalformedSort(sort);
            }

            field = parts[0].Trim();
            if (!_orderings.ContainsKey(field))
            {
                return CommonErrors.UnknownSortField(field, _allowedFields);
            }

            var parsed = parts.Length == 2 ? ParseDirection(parts[1].Trim()) : SortDirection.Ascending;
            if (parsed is null)
            {
                return CommonErrors.MalformedSort(sort);
            }

            direction = parsed.Value;
        }

        var ordered = _orderings[field](source, direction);
        return Result<IOrderedQueryable<T>>.Success(
            _tieBreaker is null ? ordered : ordered.ThenBy(_tieBreaker));
    }

    private static SortDirection? ParseDirection(string token)
        => token switch
        {
            _ when token.Equals(SortSyntax.Ascending, StringComparison.OrdinalIgnoreCase)
                => SortDirection.Ascending,
            _ when token.Equals(SortSyntax.Descending, StringComparison.OrdinalIgnoreCase)
                => SortDirection.Descending,
            _ => null,
        };
}

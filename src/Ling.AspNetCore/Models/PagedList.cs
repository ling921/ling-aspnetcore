namespace Ling.AspNetCore.Models;

/// <summary>
/// Represents a paged list of data.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class PagedList<T>
{
    /// <summary>
    /// Gets the total count of data.
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// Gets the data of current page.
    /// </summary>
    public List<T> Data { get; init; } = null!;

    /// <summary>
    /// Initialize an empty <see cref="PagedList{T}"/>.
    /// </summary>
    public PagedList()
    {
        Total = 0;
        Data = new List<T>();
    }

    /// <summary>
    /// Initialize a <see cref="PagedList{T}"/> with total count and data items.
    /// </summary>
    /// <param name="total">The total count.</param>
    /// <param name="items">The data items.</param>
    /// <exception cref="ArgumentOutOfRangeException">The total count cannot less than 0.</exception>
    /// <exception cref="ArgumentNullException">The items cannot be null.</exception>
    public PagedList(int total, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (total < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(total), "The total count can not less than 0.");
        }

        Total = 0;
        Data = items.ToList();
    }
}

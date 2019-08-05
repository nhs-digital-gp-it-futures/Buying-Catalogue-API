using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A paged list of objects
  /// </summary>
  /// <typeparam name="T">type of objects in list</typeparam>
  public sealed class PaginatedList<T>
  {
    private const int DefaultPageIndex = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;

    /// <summary>
    /// 1-based index of which page this page
    /// Defaults to 1
    /// </summary>
    public int? PageIndex { get; set; }

    /// <summary>
    /// Total number of pages based on <see cref="PageSize"/>
    /// </summary>
    public int? TotalPages { get; set; }

    /// <summary>
    /// Maximum number of items in this page
    /// Defaults to 20
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// List of items
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    /// <summary>
    /// public constructor required for JSON deserialisation
    /// </summary>
    public PaginatedList()
    {
    }

    private PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
    {
      PageIndex = pageIndex < 1 ? DefaultPageIndex : pageIndex;
      PageSize = pageSize < 1 ? DefaultPageSize : pageSize;
      TotalPages = (int)Math.Ceiling(count / (double)PageSize);

      Items = items;
    }

    /// <summary>
    /// true if there is a page of items previous to this page
    /// </summary>
    public bool HasPreviousPage
    {
      get
      {
        return (PageIndex > 1);
      }
    }

    /// <summary>
    /// true if there is a page of items after this page
    /// </summary>
    public bool HasNextPage
    {
      get
      {
        return (PageIndex < TotalPages);
      }
    }

    public static PaginatedList<T> Create(IEnumerable<T> source, int? pageIndex, int? pageSize)
    {
      int pageIndexValue = pageIndex ?? DefaultPageIndex;
      int pageSizeValue = pageSize ?? DefaultPageSize;

      var normPageIndex = pageIndexValue > 0 ? pageIndexValue - 1 : 0;
      var normPageSize = pageSizeValue > 0 ? Math.Min(pageSizeValue, MaximumPageSize) : DefaultPageSize;
      var items = source
                    .Skip(normPageIndex * normPageSize)
                    .Take(normPageSize)
                    .ToList();
      var count = source.Count();

      return new PaginatedList<T>(items, count, normPageIndex + 1, normPageSize);
    }
  }
}

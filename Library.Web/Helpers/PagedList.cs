using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Web.Helpers
{
    /// <summary>
    /// A collection for better implemented pagination.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>
    {
        /// <summary>
        /// The current page.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// The total page count.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// The current page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total overall count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// If a previous page exists.
        /// </summary>
        public bool HasPrevious => (CurrentPage > 1);

        /// <summary>
        /// If a next page exists.
        /// </summary>
        public bool HasNext => (CurrentPage < TotalPages);

        /// <inheritdoc />
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        /// <summary>
        /// Creates a PagedList based on the parameters.
        /// </summary>
        /// <param name="source">Source collection.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}

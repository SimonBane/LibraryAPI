namespace LibraryAPI.Helpers
{
    /// <summary>
    /// Parameters for different API functionallity.
    /// </summary>
    public class AuthorsResourceParameters
    {
        private const int MaxPageSize = 20;

        /// <summary>
        /// The page number you want.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        /// <summary>
        /// The page size.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        /// <summary>
        /// The author genre.
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// The search query you input.
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// A property you order by.
        /// </summary>
        public string OrderBy { get; set; } = "Name";

        /// <summary>
        /// Only the fields you want.
        /// </summary>
        public string Fields { get; set; }
    }
}

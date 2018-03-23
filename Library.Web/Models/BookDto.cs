namespace Library.Web.Models
{
    public class BookDto : LinkedResourceBaseDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }
    }
}

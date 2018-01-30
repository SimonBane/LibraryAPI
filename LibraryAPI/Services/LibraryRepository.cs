using LibraryAPI.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LibraryAPI.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly LibraryContext _context;

        public LibraryRepository(LibraryContext context)
        {
            _context = context;
        }

        public void AddAuthor(Author author)
        {
            _context.Authors.Add(author);
        }

        public void AddBookForAuthor(int authorId, Book book)
        {
            var author = GetAuthor(authorId);
            author?.Books.Add(book);
        }

        public bool AuthorExists(int authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(int authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            IQueryable<Author> query = _context.Authors;
            query = query.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);

            return query;
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<int> authorIds)
        {
            IQueryable<Author> query = _context.Authors;
            query = query.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName);

            return query;
        }

        public void UpdateAuthor(Author author)
        {
            _context.Authors.Update(author);
        }

        public Book GetBookForAuthor(int authorId, int bookId)
        {
            return _context.Books.FirstOrDefault(b => b.AuthorId == authorId && b.Id == bookId);
        }

        public IEnumerable<Book> GetBooksForAuthor(int authorId)
        {
            IQueryable<Book> query = _context.Books;
            query = query.Where(b => b.AuthorId == authorId).OrderBy(b => b.Title);

            return query;
        }

        public void UpdateBookForAuthor(Book book)
        {
            _context.Books.Update(book);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Library.Web.Entities
{
    public class LibraryContext : DbContext
    {
        private string connection;

        public LibraryContext()
        {
            connection =
                "Server = tcp:simolibraryapidbserver.database.windows.net,1433; Initial Catalog = SimoLibraryAPi_db; Persist Security Info = False; User ID = simo@simolibraryapidbserver; Password=999966665s*; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;";
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<CustomException> CustomExceptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.connection);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

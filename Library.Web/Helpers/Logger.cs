using Library.Web.Entities;
using System;
using System.Threading.Tasks;

namespace Library.Web.Helpers
{
    public class Logger : ILog
    {
        #region Private fields

        private LibraryContext _ctx = new LibraryContext();

        #endregion

        /*Lazy objects are thread safe*/
        private static readonly Lazy<Logger> instance =
            new Lazy<Logger>(() => new Logger());

        public static Logger GetInstance => instance.Value;

        public async Task LogCustomExceptionAsync(Exception ex, CustomId id = null)
        {
            CustomException exception = new CustomException(ex, id);
            _ctx.CustomExceptions.Add(exception);
            await _ctx.SaveChangesAsync();
        }

        public Task LogSendedEmailAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogPerformerRequestAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogVenueOwnerRequestAsync()
        {
            throw new NotImplementedException();
        }
    }
}

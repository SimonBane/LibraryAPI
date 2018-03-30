using System;
using System.Threading.Tasks;

namespace Library.Web.Helpers
{
    public interface ILog
    {
        Task LogCustomExceptionAsync(Exception ex, CustomId id);
        Task LogSendedEmailAsync();
        Task LogPerformerRequestAsync();
        Task LogVenueOwnerRequestAsync();
    }
}

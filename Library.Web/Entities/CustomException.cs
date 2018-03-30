using Library.Web.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Web.Entities
{
    public class CustomException : Exception
    {
        private CustomId _id;

        [Key]
        public string Id
        {
            get => _id.ToString();
            private set => _id = new CustomId(new Guid(value));
        }

        public string CustomMessage { get; set; }

        public string CustomStackTrace { get; set; }

        public string CustomInnerMessage { get; set; }

        public string CustomInnerStackTrace { get; set; }

        public DateTime DateCreated { get; set; }

        public CustomException() { }
        public CustomException(Exception ex, CustomId id = null)
        {
            CustomMessage = ex.Message;
            CustomStackTrace = ex.StackTrace;
            CustomInnerMessage = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            CustomInnerStackTrace = ex.InnerException != null ? ex.InnerException.StackTrace : String.Empty;
            DateCreated = DateTime.Now;

            _id = id ?? new CustomId();
        }
    }
}

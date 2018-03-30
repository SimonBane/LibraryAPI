using System;

namespace Library.Web.Helpers
{
    public class CustomId
    {
        private Guid _id;

        public CustomId()
        {
            this._id = Guid.NewGuid();
        }

        public CustomId(Guid guid)
        {
            this._id = guid;
        }

        public override string ToString()
        {
            return this._id.ToString();
        }
    }
}

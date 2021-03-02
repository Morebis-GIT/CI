using System;

namespace xggameplan.Model
{
    public abstract class AuditBaseModel
    {
        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        //TODO: at the time of writing it has been decided to leave it for Later.
        //      since we currently don't have any implementation to retrieve the user in the request.
        //public int CreatedBy { get; set; }
        //public int UpdatedBy { get; set; }
    }
}

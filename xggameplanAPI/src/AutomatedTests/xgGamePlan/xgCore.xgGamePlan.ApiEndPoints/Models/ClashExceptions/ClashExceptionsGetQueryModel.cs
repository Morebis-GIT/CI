using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ClashExceptions
{
    public class ClashExceptionsGetQueryModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Order OrderBy { get; set; }
        public OrderDirection OrderByDirection { get; set; }
        public int Top { get; set; }
        public int Skip { get; set; }

        public enum Order
        {
            StartDate,
            EndDate
        }

        public enum OrderDirection
        {
            Asc,
            Desc
        }
    }    
}

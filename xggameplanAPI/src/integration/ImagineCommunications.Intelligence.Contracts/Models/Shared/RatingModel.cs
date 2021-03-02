using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class RatingModel
    {
        public DateTime Time { get;  }
        public string Demographic { get;  }
        public double NoOfRatings { get;  }

        public RatingModel(DateTime time, string demographic, double noOfRatings)
        {
            Time = time;
            Demographic = demographic;
            NoOfRatings = noOfRatings;
        }
    }
}

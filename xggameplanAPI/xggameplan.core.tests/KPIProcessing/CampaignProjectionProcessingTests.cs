using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using Moq;
using NUnit.Framework;
using xggameplan.Model;

namespace xggameplan.KPIProcessing.CampaignProjection.tests
{
    [TestFixture]
    public class CampaignProjectionProcessingTests
    {
        private readonly List<string> _campaignSalesAreaList = new List<string>();
        private string _campaignSalesArea;
        private DateTime _targetStrikeWeightStartDate;
        private DateTime _targetStrikeWeightEndDate;
        private DayPartModel _dayPart;
        private Mock<Recommendation> _recommendation1;
        private Mock<Recommendation> _recommendation2;

        private int _rec1Rating;
        private int _rec2Rating;
        private int _recommendationsTotalForDayPart;

        private string _rec1Action;
        private string _rec2Action;

        private NodaTime.Duration _rec1Length;
        private NodaTime.Duration _rec2Length;

        private string _timeslice1From;
        private string _timeslice1To;
        private string _timeslice2From;
        private string _timeslice2To;

        private Recommendation _rec1obj;
        private Recommendation _rec2obj;

        [SetUp]
        public void Init()
        {
            _campaignSalesAreaList.Add("TCN91");
            _campaignSalesArea = _campaignSalesAreaList[0];
            _targetStrikeWeightStartDate = new DateTime(2018, 1, 1, 0, 0, 0);
            _targetStrikeWeightEndDate = new DateTime(2018, 1, 10, 0, 0, 0);
            _recommendation1 = new Mock<Recommendation>();
            _recommendation2 = new Mock<Recommendation>();
            _rec1Rating = 10;
            _rec2Rating = 5;
            _rec1Action = "B";                      //this is a booking and is to be added to the total
            _rec2Action = "C";                      //this is a cancellation and is to be subtracted from the total
            _rec1Length = NodaTime.Duration.FromSeconds(15);
            _rec2Length = NodaTime.Duration.FromSeconds(30);
            _timeslice1From = "00:00";
            _timeslice1To = "01:00";
            _timeslice2From = "06:00";
            _timeslice2To = "08:00";
            _recommendationsTotalForDayPart = 0;

            _dayPart = new DayPartModel()
            {
                DesiredPercentageSplit = 0,
                CurrentPercentageSplit = 10,
                Timeslices = new List<TimesliceModel>()
                {
                    new TimesliceModel() {
                        FromTime = _timeslice1From, ToTime = _timeslice1To, DowPattern = new List<string>(){"Mon", "Tues", "Wed"},
                    },
                    new TimesliceModel(){
                        FromTime = _timeslice2From, ToTime = _timeslice2To, DowPattern = new List<string>(){"Mon", "Tues", "Wed"},
                    }
                }
            };

            _ = _recommendation1.Setup(r => r.SpotRating).Returns(_rec1Rating);
            _ = _recommendation1.Setup(r => r.Action).Returns(_rec1Action);
            _ = _recommendation1.Setup(r => r.StartDateTime).Returns(_targetStrikeWeightStartDate);
            _ = _recommendation1.Setup(r => r.SalesArea).Returns(_campaignSalesArea);

            _ = _recommendation2.Setup(r => r.SpotRating).Returns(_rec2Rating);
            _ = _recommendation2.Setup(r => r.Action).Returns(_rec2Action);
            _ = _recommendation2.Setup(r => r.StartDateTime).Returns(_targetStrikeWeightStartDate);
            _ = _recommendation2.Setup(r => r.SalesArea).Returns(_campaignSalesArea);
        }

        [Test]
        public void PerformProjection_RecommendationOutsideStrikeWeightDate_ZeroRatings()
        {
            _ = _recommendation1.Setup(r => r.StartDateTime).Returns(_targetStrikeWeightEndDate.AddDays(1));    //TEST - setting to be outside of target strike weight
            _rec1obj = _recommendation1.Object;                                                             //get the object here in order to set the NodaTime Duration - otherwise get 'invalid setup of non-vitual..'
            _rec1obj.SpotLength = _rec1Length;

            var reccomendations = new List<Recommendation>() { _rec1obj };

            var rating = CampaignProjectionProcessing.ProjectRatingsForCampaignDayPart(_dayPart, reccomendations, _targetStrikeWeightStartDate, _targetStrikeWeightEndDate, _campaignSalesAreaList);

            foreach (var key in rating)
            {
                _recommendationsTotalForDayPart += (int)key.Value;
            }

            //_recommendation1.Object StartDateTime has been setup to be later than target end date, so the returned ratings total should be 0
            Assert.IsTrue(_recommendationsTotalForDayPart == 0);
        }

        [Test]
        public void PerformProjection_OneMatchingBookRecommendations_IncreasedRatings()
        {
            _rec1obj = _recommendation1.Object;
            _rec1obj.SpotLength = _rec1Length;
            var reccomendations = new List<Recommendation>() { _rec1obj };

            var rating = CampaignProjectionProcessing.ProjectRatingsForCampaignDayPart(_dayPart, reccomendations, _targetStrikeWeightStartDate, _targetStrikeWeightEndDate, _campaignSalesAreaList);

            foreach (var key in rating)
            {
                _recommendationsTotalForDayPart += (int)key.Value;
            }

            Assert.IsTrue(_recommendationsTotalForDayPart == _rec1Rating);
        }

        [Test]
        public void PerformProjection_MatchingOneBookOneCancelRecommendations_CorrectRating()
        {
            _rec1obj = _recommendation1.Object;
            _rec1obj.SpotLength = _rec1Length;
            _rec2obj = _recommendation2.Object;
            _rec2obj.SpotLength = _rec2Length;
            var reccomendations = new List<Recommendation>() { _rec1obj, _rec2obj };

            var rating = CampaignProjectionProcessing.ProjectRatingsForCampaignDayPart(_dayPart, reccomendations, _targetStrikeWeightStartDate, _targetStrikeWeightEndDate, _campaignSalesAreaList);

            foreach (var key in rating)
            {
                _recommendationsTotalForDayPart += (int)key.Value;
            }

            Assert.IsTrue(_recommendationsTotalForDayPart == _rec1Rating - _rec2Rating);
        }

        [Test]
        public void PerformProjection_RecommendationInsideStrikeWeightButNonMatchingTimesliceDOW_CorrectRating()
        {
            //4th Jan 2018 = Thursday in rec2, DOW in daypart.timeslices are Mon, Tues, Wed
            _ = _recommendation2.Setup(r => r.StartDateTime).Returns(new DateTime(2018, 1, 4, 0, 0, 0));

            _rec1obj = _recommendation1.Object;
            _rec1obj.SpotLength = _rec1Length;
            _rec2obj = _recommendation2.Object;
            _rec2obj.SpotLength = _rec2Length;
            var reccomendations = new List<Recommendation>() { _rec1obj, _rec2obj };

            var rating = CampaignProjectionProcessing.ProjectRatingsForCampaignDayPart(_dayPart, reccomendations, _targetStrikeWeightStartDate, _targetStrikeWeightEndDate, _campaignSalesAreaList);

            foreach (var key in rating)
            {
                _recommendationsTotalForDayPart += (int)key.Value;
            }

            //_recommendation2.Object has been setup to be a thursday, so the returned total should equal only to the ratings in _recommendation1.Object
            Assert.IsTrue(_recommendationsTotalForDayPart == _rec1Rating);
        }

        [Test]
        public void PerformProjection_RecommendationInsideStrikeWeightButOutsideTimeSlice_CorrectRating()
        {
            //setting _recommendation2.StartDateTime (02 hours) to be outside of the 2 daypart timeslices (00:00 to 01:00 && 06:00 to 08:00 hours)
            _ = _recommendation2.Setup(r => r.StartDateTime).Returns(new DateTime(2018, 1, 1, 2, 0, 0));

            _rec1obj = _recommendation1.Object;
            _rec1obj.SpotLength = _rec1Length;
            _rec2obj = _recommendation2.Object;
            _rec2obj.SpotLength = _rec2Length;
            var reccomendations = new List<Recommendation>() { _rec1obj, _rec2obj };

            var rating = CampaignProjectionProcessing.ProjectRatingsForCampaignDayPart(_dayPart, reccomendations, _targetStrikeWeightStartDate, _targetStrikeWeightEndDate, _campaignSalesAreaList);

            foreach (var key in rating)
            {
                _recommendationsTotalForDayPart += (int)key.Value;
            }

            //_recommendation2.Object has been setup to be outside of the time slices included in the day part, so the returned total should equal only to the ratings in _recommendation1.Object
            Assert.IsTrue(_recommendationsTotalForDayPart == _rec1Rating);
        }

        [Test]
        public void PerformProjection_MatchingRecommendationsNeighbouringTimeSlices_CorrectRating()
        {
            _ = _recommendation2.Setup(r => r.StartDateTime).Returns(new DateTime(2018, 1, 1, 2, 0, 0));

            _rec1obj = _recommendation1.Object;
            _rec1obj.SpotLength = _rec1Length;
            _rec2obj = _recommendation2.Object;
            _rec2obj.SpotLength = _rec2Length;
            var reccomendations = new List<Recommendation>() { _rec1obj, _rec2obj };

            _dayPart.Timeslices[1].FromTime = _timeslice1To;        //2 daypart timeslices set to (00:00 to 01:00 && 01:00 to 08:00 hours)
            var rating = CampaignProjectionProcessing.ProjectRatingsForCampaignDayPart(_dayPart, reccomendations, _targetStrikeWeightStartDate, _targetStrikeWeightEndDate, _campaignSalesAreaList);

            foreach (var key in rating)
            {
                _recommendationsTotalForDayPart += (int)key.Value;
            }

            //both recommendations are inside the timeslices of the day part, so the returned total should be equal to the sum of both
            Assert.IsTrue(_recommendationsTotalForDayPart == _rec1Rating - _rec2Rating);
        }
    }
}

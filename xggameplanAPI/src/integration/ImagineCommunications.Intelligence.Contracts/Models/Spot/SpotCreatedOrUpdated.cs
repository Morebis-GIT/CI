using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Spot
{
    public class SpotCreatedOrUpdated : ISpotCreatedOrUpdated
    {
        public SpotCreatedOrUpdated(
            string externalCampaignNumber, string salesArea, string groupCode, string externalSpotRef,
            DateTime startDateTime, DateTime endDateTime, TimeSpan spotLength,
            string breakType, string product, string demographic, bool clientPicked,
            string multipartSpot, string multipartSpotPosition, string multipartSpotRef,
            string requestedPositioninBreak, string actualPositioninBreak, string breakRequest, string externalBreakNo,
            bool sponsored, bool preemptable, int preemptlevel, int bookingPosition, string industryCode, string clearanceCode, decimal nominalPrice)
        {
            ExternalCampaignNumber = externalCampaignNumber;
            SalesArea = salesArea;
            GroupCode = groupCode;
            ExternalSpotRef = externalSpotRef;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            SpotLength = spotLength;
            BreakType = breakType;
            Product = product;
            Demographic = demographic;
            ClientPicked = clientPicked;
            MultipartSpot = multipartSpot;
            MultipartSpotPosition = multipartSpotPosition;
            MultipartSpotRef = multipartSpotRef;
            RequestedPositioninBreak = requestedPositioninBreak;
            ActualPositioninBreak = actualPositioninBreak;
            BreakRequest = breakRequest;
            ExternalBreakNo = externalBreakNo;
            Sponsored = sponsored;
            Preemptable = preemptable;
            Preemptlevel = preemptlevel;
            BookingPosition = bookingPosition;
            IndustryCode = industryCode;
            ClearanceCode = clearanceCode;
            NominalPrice = nominalPrice;
        }

        public string ExternalCampaignNumber { get; }

        public string SalesArea { get; }

        public string GroupCode { get; }

        public string ExternalSpotRef { get; }

        public DateTime StartDateTime { get; }

        public DateTime EndDateTime { get; }

        public TimeSpan SpotLength { get; }

        public string BreakType { get; }

        public string Product { get; }

        public string Demographic { get; }

        public bool ClientPicked { get; }

        public string MultipartSpot { get; }

        public string MultipartSpotPosition { get; }

        public string MultipartSpotRef { get; }

        public string RequestedPositioninBreak { get; }

        public string ActualPositioninBreak { get; }

        public string BreakRequest { get; }

        public string ExternalBreakNo { get; }

        public bool Sponsored { get; }

        public bool Preemptable { get; }

        public int Preemptlevel { get; }

        public int BookingPosition { get; }

        public string IndustryCode { get; }

        public string ClearanceCode { get; }

        public decimal NominalPrice { get; }
    }
}

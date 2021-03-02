using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class BookingPositionGroupProfile : Profile
    {
        public BookingPositionGroupProfile()
        {
            CreateMap<BookingPosition, Domain.PositionInBreaks.BookingPositions.BookingPosition>().ReverseMap();

            CreateMap<BookingPositionGroup, Domain.PositionInBreaks.BookingPositionGroups.Objects.BookingPositionGroup>().ReverseMap();

            CreateMap<PositionGroupAssociation, Domain.PositionInBreaks.BookingPositionGroups.Objects.PositionGroupAssociation>().ReverseMap();
        }
    }
}

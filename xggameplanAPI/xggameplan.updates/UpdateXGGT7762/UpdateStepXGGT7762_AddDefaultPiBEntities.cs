using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Persistence.RavenDb;

namespace xggameplan.Updates.UpdateXGGT7762
{
    internal class UpdateStepXGGT7762_AddDefaultPiBEntities : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT7762_AddDefaultPiBEntities(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("F8CD6C31-908B-4FBA-8F1F-F1317B299150");

        public int Sequence => 1;

        public string Name => "XGGT-7762";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var newBookingPositions = new List<BookingPosition>();
                    var newPositionGroups = new List<BookingPositionGroup>();

                    // Default Booking positions
                    newBookingPositions.Add(new BookingPosition {Position = 1, Abbreviation = "FST", BookingOrder = 1});
                    newBookingPositions.Add(new BookingPosition {Position = 2, Abbreviation = "SND", BookingOrder = 3});
                    newBookingPositions.Add(new BookingPosition {Position = 3, Abbreviation = "THD", BookingOrder = 5});
                    newBookingPositions.Add(new BookingPosition {Position = 97, Abbreviation = "APN", BookingOrder = 6});
                    newBookingPositions.Add(new BookingPosition {Position = 98, Abbreviation = "PEN", BookingOrder = 4});
                    newBookingPositions.Add(new BookingPosition {Position = 99, Abbreviation = "LST", BookingOrder = 2});

                    // Default Position Groups
                    newPositionGroups.Add(new BookingPositionGroup
                    {
                        GroupId = 1,
                        Code = "FST",
                        Description = "FST",
                        DisplayOrder = 1,
                        UserDefined = false,
                        PositionGroupAssociations = new List<PositionGroupAssociation>
                            {new PositionGroupAssociation {BookingPosition = 1, BookingOrder = 1}}
                    });
                    newPositionGroups.Add(new BookingPositionGroup
                    {
                        GroupId = 2,
                        Code = "SND",
                        Description = "SND",
                        DisplayOrder = 2,
                        UserDefined = false,
                        PositionGroupAssociations = new List<PositionGroupAssociation>
                            {new PositionGroupAssociation {BookingPosition = 2, BookingOrder = 1}}
                    });
                    newPositionGroups.Add(new BookingPositionGroup
                    {
                        GroupId = 3,
                        Code = "THD",
                        Description = "THD",
                        DisplayOrder = 3,
                        UserDefined = false,
                        PositionGroupAssociations = new List<PositionGroupAssociation>
                            {new PositionGroupAssociation {BookingPosition = 3, BookingOrder = 1}}
                    });
                    newPositionGroups.Add(new BookingPositionGroup
                    {
                        GroupId = 97,
                        Code = "APN",
                        Description = "APN",
                        DisplayOrder = 97,
                        UserDefined = false,
                        PositionGroupAssociations = new List<PositionGroupAssociation>
                            {new PositionGroupAssociation {BookingPosition = 97, BookingOrder = 1}}
                    });
                    newPositionGroups.Add(new BookingPositionGroup
                    {
                        GroupId = 98,
                        Code = "PEN",
                        Description = "PEN",
                        DisplayOrder = 98,
                        UserDefined = false,
                        PositionGroupAssociations = new List<PositionGroupAssociation>
                            {new PositionGroupAssociation {BookingPosition = 98, BookingOrder = 1}}
                    });
                    newPositionGroups.Add(new BookingPositionGroup
                    {
                        GroupId = 99,
                        Code = "LST",
                        Description = "LST",
                        DisplayOrder = 99,
                        UserDefined = false,
                        PositionGroupAssociations = new List<PositionGroupAssociation>
                            {new PositionGroupAssociation {BookingPosition = 99, BookingOrder = 1}}
                    });

                    foreach (var bookingPosition in newBookingPositions)
                    {
                        session.Store(bookingPosition);
                    }

                    foreach (var positionGroup in newPositionGroups)
                    {
                        session.Store(positionGroup);
                    }

                    session.SaveChanges();
                }
            }
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}

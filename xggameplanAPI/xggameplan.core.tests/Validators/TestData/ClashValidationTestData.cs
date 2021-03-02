using System;
using xggameplan.core.Validators.Models;
using xggameplan.Model;

namespace xggameplan.core.tests.Validators.TestData
{
    public sealed class ClashValidationTestData
    {
        public const string LastLeafClashExternalReference = "LAST_LEAF_CLASH_EXTERNAL_REFERENCE";
        public const string ParentLeafClashExternalReference = "PARENT_LEAF_CLASH_EXTERNAL_REFERENCE";
        public const string FirstPlainClashExternalReference = "FIRST_PLAIN_CLASH_EXTERNAL_REFERENCE";
        public const string SecondPlainClashExternalReference = "SECOND_PLAIN_CLASH_EXTERNAL_REFERENCE";

        public static readonly object[] ClashHasNoChildClashesValidationData =
        {
            new object[]
            {
                LastLeafClashExternalReference,
                CustomValidationResult.Success()
            },
            new object[]
            {
                ParentLeafClashExternalReference,
                CustomValidationResult.Failed()
            }
        };

        public static readonly object[] ClashIsNotLinkedToActiveProductTestData =
        {
            new object[]
            {
                FirstPlainClashExternalReference,
                CustomValidationResult.Success()
            },
            new object[]
            {
                SecondPlainClashExternalReference,
                CustomValidationResult.Failed()
            }
        };

        public static readonly Clash[] BunchOfClashes =
        {
            new Clash {ParentExternalidentifier = ParentLeafClashExternalReference},
            new Clash {ParentExternalidentifier = string.Empty}
        };

        public static readonly Product[] BunchOfProducts =
        {
            new Product {EffectiveEndDate = DateTime.UtcNow.AddDays(-1), ClashCode = FirstPlainClashExternalReference},
            new Product {EffectiveEndDate = DateTime.UtcNow.AddDays(1), ClashCode = SecondPlainClashExternalReference}
        };
    }
}

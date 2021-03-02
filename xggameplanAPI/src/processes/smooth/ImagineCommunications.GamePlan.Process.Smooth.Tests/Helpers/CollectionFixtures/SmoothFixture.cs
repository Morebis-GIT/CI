using System;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures
{
    /// <summary>
    /// Shared context. See https://xunit.net/docs/shared-context
    /// </summary>
    public class SmoothFixture
    {
        /// <summary>
        /// A unique value used to segment repositories so they do not clash.
        /// </summary>
        public Guid RepositorySegmentationSalt { get; } = Guid.NewGuid();

        /// <summary>
        /// The number of milliseconds that an object in the test repository
        /// should live.
        /// </summary>
        public double ObjectTTL { get; } = TimeSpan.FromMinutes(10).TotalMilliseconds;

        public static readonly ProgrammeResult[]
            _recommendationsAreSetWithNoSponsorshipRulesExpectedResults = new[] {
                new ProgrammeResult
                {
                    ProgrammeExternalReference = "Prog1",
                    Breaks = new [] {
                        new BreakResult
                        {
                            ExternalReference = "Prog1-break1",
                            Products = new []{ "Apple_MacBookPro13","Dell_XPS15","KFC_FilletBurger","KFC_ZingerBurger","Pepsi_NormalPepsi","PetAtHome_DogFood","Subway_Meatball" },
                            SpotsInPositionOrder = new []{ "101-P1-B1-KFC_FilletBurger","113-P1-B1-Dell_XPS15","122-P1-B1-PetAtHome_DogFood","117-P1-B1-Apple_MacBookPro13","150-P1-B3-KFC_ZingerBurger","104-P1-B1-Subway_Meatball","159-P1-B3-Pepsi_NormalPepsi","135-P1-B2-Pepsi_NormalPepsi" }
                        },
                        new BreakResult
                        {
                            ExternalReference = "Prog1-break2",
                            Products = new []{ "Apple_MacBookPro13","Dell_XPS17","EE_Iphone11Max","PetAtHome_DogFood","PetAtHome_RabbitFood","Subway_BMT","Subway_Chicken" },
                            SpotsInPositionOrder = new []{ "138-P1-B2-Dell_XPS17","105-P1-B1-Subway_BMT","145-P1-B2-EE_Iphone11Max","148-P1-B2-PetAtHome_RabbitFood","165-P1-B3-Apple_MacBookPro13","130-P1-B2-Subway_Chicken","106-P1-B1-Subway_Chicken","146-P1-B2-PetAtHome_DogFood" }
                        },
                        new BreakResult
                        {
                            ExternalReference = "Prog1-break3",
                            Products = new []{ "Apple_MacBookPro15","Coca Cola_ZeroCoke","EE_IphoneX","KFC_chickenWings","Pepsi_PepsiMax","PetAtHome_CatFood","PetAtHome_DogFood","Subway_BMT" },
                            SpotsInPositionOrder = new []{ "168-P1-B3-EE_IphoneX","170-P1-B3-PetAtHome_DogFood","153-P1-B3-Subway_BMT","166-P1-B3-Apple_MacBookPro15","171-P1-B3-PetAtHome_CatFood","103-P1-B1-KFC_chickenWings","156-P1-B3-Coca Cola_ZeroCoke","158-P1-B3-Pepsi_PepsiMax" }
                        },
                    },
                },
                new ProgrammeResult
                {
                    ProgrammeExternalReference = "Prog2",
                    Breaks = new [] {
                        new BreakResult
                        {
                            ExternalReference = "Prog2-break1",
                            Products = new []{ "Dell_XPS13","Dell_XPS15","EE_IphoneX","KFC_chickenWings","KFC_FilletBurger","Pepsi_NormalPepsi","Pepsi_PepsiMax","PetAtHome_CatFood" },
                            SpotsInPositionOrder = new []{ "173-P2-B1-KFC_FilletBurger","185-P2-B1-Dell_XPS15","183-P2-B1-Pepsi_NormalPepsi","195-P2-B1-PetAtHome_CatFood","192-P2-B1-EE_IphoneX","235-P2-B3-Dell_XPS13","182-P2-B1-Pepsi_PepsiMax","199-P2-B2-KFC_chickenWings" }
                        },
                        new BreakResult
                        {
                            ExternalReference = "Prog2-break2",
                            Products = new []{ "Apple_MacBookPro15","Coca Cola_DietCoke","Dell_XPS15","EE_Iphone11","KFC_ZingerBurger","Subway_Chicken","Subway_Meatball" },
                            SpotsInPositionOrder = new []{ "200-P2-B2-Subway_Meatball","191-P2-B1-EE_Iphone11","205-P2-B2-Coca Cola_DietCoke","178-P2-B1-Subway_Chicken","224-P2-B3-Subway_Meatball","238-P2-B3-Apple_MacBookPro15","209-P2-B2-Dell_XPS15","222-P2-B3-KFC_ZingerBurger" }
                        },
                        new BreakResult
                        {
                            ExternalReference = "Prog2-break3",
                            Products = new []{ "Apple_MacBookAir","Apple_MacBookPro15","Coca Cola_NormalCoke","EE_Iphone11","Pepsi_CherryPepsi","PetAtHome_CatFood","Subway_Chicken" },
                            SpotsInPositionOrder = new []{ "226-P2-B3-Subway_Chicken","239-P2-B3-EE_Iphone11","184-P2-B1-Pepsi_CherryPepsi","190-P2-B1-Apple_MacBookPro15","232-P2-B3-Pepsi_CherryPepsi","236-P2-B3-Apple_MacBookAir","243-P2-B3-PetAtHome_CatFood","227-P2-B3-Coca Cola_NormalCoke" }
                        },
                    },
                },
                new ProgrammeResult
                {
                    ProgrammeExternalReference = "Prog3",
                    Breaks = new [] {
                        new BreakResult
                        {
                            ExternalReference = "Prog3-break1",
                            Products = new []{ "Apple_MacBookPro13","Coca Cola_DietCoke","Dell_XPS15","KFC_FilletBurger","Pepsi_PepsiMax","PetAtHome_DogFood","Subway_Chicken" },
                            SpotsInPositionOrder = new []{ "257-P3-B1-Dell_XPS15","245-P3-B1-KFC_FilletBurger","266-P3-B1-PetAtHome_DogFood","301-P3-B3-Coca Cola_DietCoke","261-P3-B1-Apple_MacBookPro13","302-P3-B3-Pepsi_PepsiMax","293-P3-B3-KFC_FilletBurger","298-P3-B3-Subway_Chicken" }
                        },
                        new BreakResult
                        {
                            ExternalReference = "Prog3-break2",
                            Products = new []{ "Apple_MacBookAir","Apple_MacBookPro13","Coca Cola_ZeroCoke","Dell_XPS15","EE_Iphone11","EE_Iphone11Max","Pepsi_CherryPepsi","Subway_Meatball" },
                            SpotsInPositionOrder = new []{ "276-P3-B2-Coca Cola_ZeroCoke","260-P3-B1-Apple_MacBookAir","289-P3-B2-EE_Iphone11Max","281-P3-B2-Dell_XPS15","272-P3-B2-Subway_Meatball","280-P3-B2-Pepsi_CherryPepsi","287-P3-B2-EE_Iphone11","309-P3-B3-Apple_MacBookPro13" }
                        },
                        new BreakResult
                        {
                            ExternalReference = "Prog3-break3",
                            Products = new []{ "Apple_MacBookAir","Coca Cola_DietCoke","EE_Iphone11","EE_Iphone11Max","EE_IphoneX","KFC_chickenWings","Subway_Meatball" },
                            SpotsInPositionOrder = new []{ "295-P3-B3-KFC_chickenWings","284-P3-B2-Apple_MacBookAir","277-P3-B2-Coca Cola_DietCoke","271-P3-B2-KFC_chickenWings","296-P3-B3-Subway_Meatball","312-P3-B3-EE_IphoneX","313-P3-B3-EE_Iphone11Max","311-P3-B3-EE_Iphone11" }
                        },
                    },
                },
            };

        public ref readonly ProgrammeResult[] RecommendationsAreSetWithNoSponsorshipRulesExpectedResults() =>
            ref _recommendationsAreSetWithNoSponsorshipRulesExpectedResults;
    }
}

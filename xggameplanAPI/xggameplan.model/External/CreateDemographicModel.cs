namespace xggameplan.Model
{
    public class CreateDemographicModel
    {
        //public int Id { get; set; }
        public string ExternalRef { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int DisplayOrder { get; set; }
        public bool Gameplan { get; set; }

        public CreateDemographicModel()
        {
            Gameplan = true;      // Default
        }
    }
}

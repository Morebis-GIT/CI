namespace xgCore.xgGamePlan.ApiEndPoints.Models.Demographics
{
    /// <summary>
    /// Represents a Demographic within GamePlan
    /// </summary>
    public class Demographic
    {
        public int Id { get; set; }
        public string ExternalRef { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int DisplayOrder { get; set; }
        public bool Gameplan { get; set; } = true;
    }
}

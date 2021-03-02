namespace xggameplan.Model
{
    public class CreateAnalysisGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CreateAnalysisGroupFilterModel Filter { get; set; }
    }
}

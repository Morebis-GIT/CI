namespace xggameplan.model.External
{
    public class ProgrammeCategoryHierarchyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalRef { get; set; }
        public string ParentExternalRef { get; set; }
    }
}

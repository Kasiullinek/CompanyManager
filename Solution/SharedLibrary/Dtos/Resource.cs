namespace SharedLibrary.Dtos
{
    public class Resource
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string AssignedToEmail { get; set; } = "";
        public bool IsCompleted { get; set; } = false;
    }

}

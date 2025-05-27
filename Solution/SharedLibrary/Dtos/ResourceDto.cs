using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Dtos
{
    public class ResourceDto
    {
        public string Id { get; set; } = "";

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string AssignedToEmail { get; set; } = "";
        public bool IsCompleted { get; set; } = false;
    }
}

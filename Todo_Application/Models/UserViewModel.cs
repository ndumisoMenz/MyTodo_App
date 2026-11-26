using System.ComponentModel.DataAnnotations;

namespace Todo_Application.Models
{
    public class UserViewModel
    {
        public string Id {  get; set; }
        public string Email { get; set; }
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        public string? Role { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Todo_Application.Models
{
    public class Todo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }

        [Required(ErrorMessage ="Please enter task title")]
        public string Title { get; set; }
        [Display(Name="is Completed")]
        public bool isComplted {  get; set; }=false;
        [Required(ErrorMessage ="Please provide breif of The task")]
        public string Description { get; set; }
        public DateTime DateCreated {  get; set; }=DateTime.Now;
        [Required(ErrorMessage ="Please enter task due date")]
        [FutureDate]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}

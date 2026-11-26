using System.ComponentModel.DataAnnotations;

namespace Todo_Application.Models
{
    public class FutureDateAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Please enter task due date");
            var dueDate=(DateTime?)value;
            if (dueDate < DateTime.Now)
            {
                return new ValidationResult("Due date cannot be before current date and time");
            }
            return ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is Required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage ="your name is required")]
        [Display(Name="Submitted By")]
        public string SubmittedBy { get; set; }

        [Display(Name ="Submitted On")]
        public DateTime datetime { get; set; }

        public DateTime? UpdatedOn { get; set; }
        //The ? makes it nullable. New complaints have never been updated — so this is null until admin changes the status.

        public string? AdminNote { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace inven_edu.Models.ViewModels
{
    /// <summary>
    /// View model for displaying and editing categories
    /// </summary>
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Number of Items")]
        public int ItemCount { get; set; }
    }
}

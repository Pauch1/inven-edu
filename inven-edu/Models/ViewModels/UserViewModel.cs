using System.ComponentModel.DataAnnotations;

namespace inven_edu.Models.ViewModels
{
    /// <summary>
    /// View model for displaying user information
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Total Issuances")]
        public int TotalIssuances { get; set; }
    }

    /// <summary>
    /// View model for user management
    /// </summary>
    public class ManageUsersViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public string? SearchTerm { get; set; }
        public bool? ActiveOnly { get; set; }
    }
}

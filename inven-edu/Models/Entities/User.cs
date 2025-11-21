using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace inven_edu.Models.Entities
{
    /// <summary>
    /// Represents a user in the InvenEdu system
    /// Extends IdentityUser to include additional user information
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the user's first name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the user account was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets whether the user account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation property for issuance records associated with this user
        /// </summary>
        public virtual ICollection<IssuanceRecord> IssuanceRecords { get; set; } = new List<IssuanceRecord>();

        /// <summary>
        /// Gets the full name of the user
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";
    }
}

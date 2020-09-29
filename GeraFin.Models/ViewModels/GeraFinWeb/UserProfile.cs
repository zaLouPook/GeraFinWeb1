using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class UserProfile
    {
        [Key]
        public int UserProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string OldPassword { get; set; }
        public string ProfilePicture { get; set; } = "/upload/blank-person.png";
        public string ApplicationUserId { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
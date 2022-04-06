using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
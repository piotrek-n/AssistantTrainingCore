using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.Models
{

    public class ResetPasswordModel
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        public string Token { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CloudComputingA3.Models 
{ 


    public class RegisterViewModel
    {
        [Required, Display(Name = "Email")]
        public string Email { get; set; }

        [Required, Display(Name = "Username")]
        public string Username { get; set; }

        [Required, Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "User Image")]
        public IFormFile UserImage { get; set; }
    }
}

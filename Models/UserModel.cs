using System.ComponentModel.DataAnnotations;

namespace Quiz_Management.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
    public class UserDropDownModel()
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
    public class UserRegisterModel
    {
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        public string MobileNo { get; set; }
    }
}

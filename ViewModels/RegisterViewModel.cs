using System.ComponentModel.DataAnnotations;

namespace MvcCookieAuthSample.ViewModels
{
    public class RegisterViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email{get;set;}
        [DataType(DataType.Password)]
        [Required]
        public string Password{get;set;}
        [DataType(DataType.Password)]
        [Required]

        public string ConfirmedPassword{get;set;} 
    }
}
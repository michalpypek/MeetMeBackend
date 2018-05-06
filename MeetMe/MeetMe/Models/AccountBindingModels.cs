using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MeetMe.Models
{
    // Models used as parameters to AccountController actions.

    //public class AddExternalLoginBindingModel
    //{
    //    [Display(Name = "External access token")]
    //    public string ExternalAccessToken { get; set; }
    //}

    //public class ChangePasswordBindingModel
    //{

    //    public string OldPassword { get; set; }

    //    [StringLength(100, ErrorMessage = "szur pierdolnelo", MinimumLength = 6)]
    //    public string NewPassword { get; set; }

    //    [Display(Name = "Confirm new password")]
    //    [Compare("NewPassword", ErrorMessage = "wykurwilo sie")]
    //    public string ConfirmPassword { get; set; }
    //}

    public class RegisterBindingModel
    {
		public string Token { get; set; }
    }

    //public class RegisterExternalBindingModel
    //{
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}

    //public class RemoveLoginBindingModel
    //{
    //    [Display(Name = "Login provider")]
    //    public string LoginProvider { get; set; }

    //    [Display(Name = "Provider key")]
    //    public string ProviderKey { get; set; }
    //}

    //public class SetPasswordBindingModel
    //{
    //    [Display(Name = "New password")]
    //    public string NewPassword { get; set; }

    //    [Display(Name = "Confirm new password")]
    //    [Compare("NewPassword", ErrorMessage = "Tam jeblo")]
    //    public string ConfirmPassword { get; set; }
    //}
}

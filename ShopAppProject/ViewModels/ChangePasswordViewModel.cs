// ChangePasswordViewModel.cs
using ShopAppProject.Data;
//test

namespace ShopAppProject.Data
{
    public class ChangePasswordViewModel
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}

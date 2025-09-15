namespace ApplicationLogic.ViewModels
{
    public class ChangePasswordBody
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}

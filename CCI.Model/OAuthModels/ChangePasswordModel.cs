namespace CCI.Model.OAuthModels
{
    public class ChangePasswordModel
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}

namespace GeraFin.Models.ViewModels.Manage
{
    public class TwoFactorAuthentication
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }
    }
}

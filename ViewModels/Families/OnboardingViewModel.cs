namespace Moonatna.ViewModels.Families;

public class OnboardingViewModel
{
    public string? CreateName { get; set; }
    public string? JoinCode { get; set; }
    public string? ErrorKey { get; set; }   // e.g. "Family.Join.InvalidCode"
}

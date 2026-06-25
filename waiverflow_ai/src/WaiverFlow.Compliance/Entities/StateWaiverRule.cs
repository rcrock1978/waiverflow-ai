using WaiverFlow.Shared.Entities;

namespace WaiverFlow.Compliance.Entities;

public class StateWaiverRule : Entity
{
    public string StateCode { get; set; } = string.Empty;
    public string WaiverType { get; set; } = string.Empty;
    public bool AllowsPartialWaiver { get; set; }
    public bool RequiresNotarization { get; set; }
    public int StatutoryGracePeriodDays { get; set; }
    public string? Notes { get; set; }
}

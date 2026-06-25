using WaiverFlow.Compliance.Entities;

namespace WaiverFlow.Compliance.Services;

public class StateWaiverRuleService
{
    private readonly List<StateWaiverRule> _rules =
    [
        new() { StateCode = "AL", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "AK", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "AZ", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "AR", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "CA", WaiverType = "unconditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "CO", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "CT", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "DE", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "FL", WaiverType = "conditional", AllowsPartialWaiver = false, RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "GA", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "HI", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "ID", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "IL", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "IN", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "IA", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "KS", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "KY", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "LA", WaiverType = "unconditional", AllowsPartialWaiver = true,  RequiresNotarization = true,  StatutoryGracePeriodDays = 60  },
        new() { StateCode = "ME", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "MD", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "MA", WaiverType = "unconditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "MI", WaiverType = "unconditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "MN", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "MS", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "MO", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "MT", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "NE", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "NV", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "NH", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "NJ", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "NM", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "NY", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = true,  StatutoryGracePeriodDays = 120 },
        new() { StateCode = "NC", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "ND", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "OH", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "OK", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "OR", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "PA", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "RI", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 120 },
        new() { StateCode = "SC", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "SD", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "TN", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "TX", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 30  },
        new() { StateCode = "UT", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "VT", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "VA", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "WA", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 60  },
        new() { StateCode = "WV", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "WI", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "WY", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  },
        new() { StateCode = "DC", WaiverType = "conditional", AllowsPartialWaiver = true,  RequiresNotarization = false, StatutoryGracePeriodDays = 90  }
    ];

    public Task<List<StateWaiverRule>> GetAllAsync() => Task.FromResult(_rules);
    public Task<StateWaiverRule?> GetByStateAsync(string stateCode) =>
        Task.FromResult(_rules.FirstOrDefault(r => r.StateCode == stateCode));
}

using Microsoft.AspNetCore.Mvc;
using WaiverFlow.Compliance.Services;

namespace WaiverFlow.Compliance.Api;

[ApiController]
[Route("api/v1/admin/state-rules")]
public class StateRulesController : ControllerBase
{
    private readonly StateWaiverRuleService _rules;

    public StateRulesController(StateWaiverRuleService rules) => _rules = rules;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _rules.GetAllAsync());

    [HttpGet("{stateCode}")]
    public async Task<IActionResult> GetByState(string stateCode)
    {
        var rule = await _rules.GetByStateAsync(stateCode.ToUpper());
        return rule is null ? NotFound() : Ok(rule);
    }
}

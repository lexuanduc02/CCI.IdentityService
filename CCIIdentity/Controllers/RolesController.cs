using CCI.Model;
using CCI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCIIdentity;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "admin_recruiter")]
public class RolesController : Controller
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPut("role-assign")]
    public async Task<IActionResult> RoleAssign([FromBody] RoleAssignRequest request)
    {
        var result = await _roleService.RoleAssign(request);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}

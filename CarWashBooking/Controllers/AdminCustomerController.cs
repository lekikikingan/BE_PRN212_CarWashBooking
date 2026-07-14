using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

[ApiController]
[Route("api/admin/customers")]
[Authorize(Roles = "Admin")]
public class AdminCustomerController : ControllerBase
{
    private readonly IAdminCustomerService _service;

    public AdminCustomerController(IAdminCustomerService service) => _service = service;
}

using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

[ApiController]
[Route("api/admin/transactions")]
[Authorize(Roles = "Admin")]
public class AdminTransactionController : ControllerBase
{
    private readonly IAdminTransactionService _service;

    public AdminTransactionController(IAdminTransactionService service) => _service = service;
}

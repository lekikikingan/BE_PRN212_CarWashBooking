using CarWashBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;
}

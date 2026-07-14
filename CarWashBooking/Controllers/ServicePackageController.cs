using CarWashBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicePackageController : ControllerBase
{
    private readonly IServicePackageService _service;

    public ServicePackageController(IServicePackageService service) => _service = service;
}

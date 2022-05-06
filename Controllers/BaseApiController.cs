using Microsoft.AspNetCore.Mvc;

namespace ApiVersioning.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]", Order = 0)]
public class BaseApiController : ControllerBase
{

}
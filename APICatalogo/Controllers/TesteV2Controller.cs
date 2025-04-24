using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace APICatalogo.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/teste")]
[ApiVersion("2.0")]
public class TesteV2Controller : Controller
{
    [HttpGet]
    public string GetVersion()
    {
        return "TesteV2 - GET - Api Versão 2.0";
    }
}

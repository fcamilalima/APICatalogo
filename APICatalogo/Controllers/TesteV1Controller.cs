using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace APICatalogo.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/teste")]
[ApiVersion("1.0", Deprecated = true)]
public class TesteV1Controller : Controller
{
    [HttpGet]
    public string GetVersion()
    {
        return "TesteV1 - GET - Api Versão 1.0";
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;
[Route("api/teste")]
[ApiController]
[ApiVersion(3)]
[ApiVersion(4)]
public class TesteV3Controller : Controller
{
    [HttpGet]
    [MapToApiVersion(3)]
    public string GetAction3()
    {
        return "Version3 - GET - Api Versão 3.0";
    }

    [HttpGet]
    [MapToApiVersion(4)]
    public string GetAction4()
    {
        return "Version4 - GET - Api Versão 4.0";
    }
}

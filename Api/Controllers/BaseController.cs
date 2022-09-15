using Infra.Crosscutting.Logs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [EnableCors("CORSPOLICY")]
    [Produces("application/json")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class BaseController : Controller
    {
        protected readonly LogModel _log;

        public BaseController()
        {
            this._log = new LogModel();
        }
    }
}
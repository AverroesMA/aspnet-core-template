using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Template.Controllers
{
    public class HomeController : ApiController
    {
        [Authorize]
        public ActionResult<string> Privacy()
        {
            return Ok("hey");
        }
    }
}

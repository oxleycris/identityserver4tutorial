using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OxApi.Controllers
{
    [Produces("application/json")]
    [Route("identity")]
    [Authorize]
    public class IdentityController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var objectList = new List<object>();
            var rnd = new Random();

            for (var i = 0; i < 5; i++)
            {
                objectList.Add(new { First = rnd.Next(1, 13), Second = rnd.Next(1, 13) });

            }

            return new JsonResult(objectList);
        }
    }
}

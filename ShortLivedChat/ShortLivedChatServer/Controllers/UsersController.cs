using IdentityServer4.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShortLivedChatServer.IdentityServerConfig;

namespace ShortLivedChatServer.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        [HttpPost]
        public ActionResult Post([FromBody] TestUser newUser)
        {
            ISConfig.AddUser(newUser);
            return Ok();
        }
    }
}
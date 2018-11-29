using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LenesKlinik.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }


        // POST api/work
        [HttpPost("{clearPass}")]
        public ActionResult<User> Post([FromBody] User user, string clearPass)
        {
            try
            {
                return _service.CreateUser(user, clearPass);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }




    }
}
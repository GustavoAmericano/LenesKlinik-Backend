using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.Entities;
using LenesKlinik.RestApi.DTO;
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
        public ActionResult<SafeUser> Post([FromBody] User user, string clearPass)
        {
            try
            {
                user = _service.CreateUser(user, clearPass);
                return new SafeUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    Customer = user.Customer,
                };
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{clearPass}/{newPass?}")]
        public ActionResult<SafeUser> Put([FromBody] User user, string clearPass, string newPass)
        {
            try
            {
                user = _service.UpdateUser(user, clearPass, newPass);
                return new SafeUser
                {
                    Id = user.Id,
                    Customer = user.Customer,
                    Email = user.Email
                };
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }





    }
}
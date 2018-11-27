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
    public class WorkController : ControllerBase
    {
        private IWorkService _service;

        public WorkController(IWorkService service)
        {
            _service = service;
        }
        //GET api/values
        [HttpGet]
        public ActionResult<Work> Get()
        {
            return new Work();
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        // POST api/work
        [HttpPost]
        public ActionResult<Work> Post([FromBody] Work work)
        {
            try
            {
                return _service.CreateWork(work);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}

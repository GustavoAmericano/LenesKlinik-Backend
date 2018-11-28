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
        public ActionResult<List<Work>> Get()
        {
            try
            {
                return _service.GetAllWork().ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/work/5
        [HttpGet("{id}")]
        public ActionResult<Work> Get(int id)
        {
            return _service.GetWorkById(id);
        }

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

        // PUT api/work/5
        [HttpPut("{id}")]
        public ActionResult<Work> Put(int id, [FromBody] Work work)
        {
            try
            {
                return _service.UpdateWork(id, work);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/work/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _service.DeleteWork(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

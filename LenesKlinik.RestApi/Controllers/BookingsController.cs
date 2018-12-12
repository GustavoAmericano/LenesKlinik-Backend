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
    public class BookingsController : ControllerBase
    {
        private IBookingService _service;

        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        // POST api/booking
        [HttpGet]
        public ActionResult<List<DateSessions>> Get([FromQuery] dateWithDuration dto)
        {
            try
            {
                return _service.GetAvailableBookings(dto.Date, dto.WorkId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetByUser/{id}")]
        public ActionResult<List<Booking>> GetByUser(int id)
        {
            try
            {
                return _service.GetBookingsByCustomerId(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{date}")]
        public ActionResult<List<BookingInfo>> Get(DateTime date)
        {
            try
            {
                return _service.GetBookingsForWeek(date);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult<Booking> Post([FromBody] Booking booking)
        {
            try
            {
                return _service.SaveBooking(booking);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _service.DeleteBooking(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
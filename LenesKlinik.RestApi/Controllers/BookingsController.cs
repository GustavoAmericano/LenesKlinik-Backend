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
        public ActionResult<List<AvailableSessionsForDate>> Get([FromQuery] dateWithDuration dto)
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
    }
}
using System;
using System.Collections.Generic;

namespace LenesKlinik.Core.Entities
{
    public class BookingInfo
    {
        public DateTime Date { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
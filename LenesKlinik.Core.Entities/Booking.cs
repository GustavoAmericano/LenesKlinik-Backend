using System;

namespace LenesKlinik.Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public Work Work{ get; set; }
        public Customer Customer { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
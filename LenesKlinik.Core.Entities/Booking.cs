using System;

namespace LenesKlinik.Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int WorkId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime endTime { get; set; }
    }
}
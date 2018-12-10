using System;
using System.Collections.Generic;

namespace LenesKlinik.Core.Entities
{
    public class Customer
    {
        //Personal information
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public List<Booking> Bookings { get; set; }
        public User User { get; set; }
        public DateTime Birthdate { get; set; }
        public int PhoneNumber { get; set; }
    }
}
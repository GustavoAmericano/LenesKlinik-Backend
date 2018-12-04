using System;
using System.Collections.Generic;
using LenesKlinik.Core.DomainServices;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class BookingService : IBookingService
    {
        private IBookingRepository _repo;

        public BookingService(IBookingRepository repo)
        {
            _repo = repo;
        }

        public DateTime[] GetWeek(DateTime date)
        {
            DateTime[] week = new DateTime[7];

            var monday = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            for (int i = 0; i < 7; i++)
            {
                var weekDay = monday.AddDays(i);
                week[i] = weekDay;
            }

            return week;
        }
    }

}
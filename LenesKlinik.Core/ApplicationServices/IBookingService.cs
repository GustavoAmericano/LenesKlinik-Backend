using System;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IBookingService
    {
        DateTime[] GetWeek(DateTime date);
    }
}
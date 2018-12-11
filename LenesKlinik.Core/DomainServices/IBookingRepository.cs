using System;
using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.DomainServices
{
    public interface IBookingRepository
    {
        /// <summary>
        /// Saves the booking entity to the DB.
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>Returns the saved entity.</returns>
        Booking SaveBooking(Booking booking);

        /// <summary>
        /// Gets all bookings which startDate is the same as the input date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Returns a list of Booking entities, which startDate is equal to the input date.</returns>
        List<Booking> GetBookingsByDate(DateTime date);

        /// <summary>
        /// Gets all bookings related to the customer with the given Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>A list of Bookings</returns>
        List<Booking> GetBookingsByCustomerId(int customerId);
    }
}
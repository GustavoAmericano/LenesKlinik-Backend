using System;
using System.Collections.Generic;

namespace LenesKlinik.Core.Entities
{
    public class AvailableSessionsForDate
    {
        public DateTime Date { get; set; }
        public List<AvailableSession> AvailableSessions { get; set; }
    }
    
    //TODO change name
}
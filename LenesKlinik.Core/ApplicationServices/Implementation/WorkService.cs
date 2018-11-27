using System;
using System.Collections.Generic;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class WorkService : IWorkService
    {
        private IWorkRepository _repo;

        public WorkService(IWorkRepository repo)
        {
            _repo = repo;
        }

        public Work CreateWork(Work work)
        {
            if(string.IsNullOrEmpty(work.Description)) throw new ArgumentException("Description empty or null!");
            if(string.IsNullOrEmpty(work.Title)) throw new ArgumentException("Title empty or null!");
            if(work.Duration <= 0) throw new ArgumentOutOfRangeException("Duration","Duration cannot be 0 or less!");
            if(work.Price <= 0) throw new ArgumentOutOfRangeException("Price", "Price cannot be 0 or less!");
            try
            {
                return _repo.CreateWork(work);
            }
            catch (Exception)
            {
                throw new  Exception("Error storing entity in database!");
            }
        }

        public IEnumerable<Work> GetAllWork()
        {
            try
            {
                return _repo.GetAllWork();
            }
            catch (Exception)
            {
                throw new Exception("Error loading entities from database!");
            }
        }
    }
}
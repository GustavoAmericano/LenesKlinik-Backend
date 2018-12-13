using System;
using System.Collections.Generic;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;

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
            try
            {
                ValidateWork(work);
                return _repo.CreateWork(work);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to save the service.");
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
                throw new Exception("An Error occured trying to fetch the services.");
            }
        }

        
        public Work GetWorkById(int workId)
        {
            Work work;
            try
            {
                work = _repo.GetWorkById(workId);
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to fetch the service.");
            }
            if (work == null) throw new ArgumentException($"No entity found with id {workId}!");
            return work;
        }

        public void DeleteWork(int workId)
        {
            try
            {
                _repo.DeleteWork(workId);
            }
            catch (InvalidOperationException e)
            {
                throw new Exception(e.Message);
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to delete the service.");
            }
        }

        public Work UpdateWork(int workId, Work work)
        {
            if(workId != work.Id) throw new ArgumentException("Id mismatch");
            try
            {
                ValidateWork(work);
                return _repo.UpdateWork(work);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to save the service.");
            }
        }


        public void ValidateWork(Work work)
        {
            if (string.IsNullOrEmpty(work.Description)) throw new ArgumentException("Description empty or null!");
            if (string.IsNullOrEmpty(work.Title)) throw new ArgumentException("Title empty or null!");
            if (work.Duration <= 0) throw new ArgumentException("Duration cannot be 0 or less!");
            if (work.Price <= 0) throw new ArgumentException("Price cannot be 0 or less!");
        }
    }
}
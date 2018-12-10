using System;
using System.Collections.Generic;
using System.Linq;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Parsing;

namespace LenesKlinik.Data.Repositories
{
    public class WorkRepository : IWorkRepository
    {
        private readonly DataContext _ctx;

        public WorkRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public Work CreateWork(Work work)
        {

            try
            {
                _ctx.Attach(work).State = EntityState.Added;
                _ctx.SaveChanges();
                return work;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public IEnumerable<Work> GetAllWork()
        {
            try
            {
                return _ctx.Work;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public void DeleteWork(int workId)
        {
            try
            {
                Work work = _ctx.Work.FirstOrDefault(wo => wo.Id == workId) ?? throw new InvalidOperationException($"No entity with id {workId}");
                _ctx.Bookings.RemoveRange(_ctx.Bookings.Where(book => book.Work.Id == work.Id));
                _ctx.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public Work UpdateWork(Work work)
        {
            _ctx.Work.Remove(_ctx.Work.FirstOrDefault(wo => wo.Id == work.Id)
                             ?? throw new InvalidOperationException($"No entity with id {work.Id}"));
            try
            {
                _ctx.Attach(work).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception();
            }
            return work;
        }

        public Work GetWorkById(int workId)
        {
            try
            {
                return _ctx.Work.FirstOrDefault(work => work.Id == workId);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}
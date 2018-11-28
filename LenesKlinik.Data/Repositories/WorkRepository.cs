using System;
using System.Collections.Generic;
using System.Linq;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LenesKlinik.Data.Repositories
{
    public class WorkRepository : IWorkRepository
    {
        private DataContext _ctx;

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
            _ctx.Work.Remove(_ctx.Work.FirstOrDefault(wo => wo.Id == workId)
                             ?? throw new InvalidOperationException($"No entity with id {workId}"));
            _ctx.SaveChanges();
        }
    }
}
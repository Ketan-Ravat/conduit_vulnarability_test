using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface ICompletedPMTriggerRepository
    {
        Task<int> Insert(CompletedPMTriggers entity);
        Task<int> Update(CompletedPMTriggers entity);
    }
}

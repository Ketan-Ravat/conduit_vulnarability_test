using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IPMTriggersRemarksRepository
    {
        Task<int> Insert(PMTriggersRemarks entity);
        Task<int> Update(PMTriggersRemarks entity);
    }
}

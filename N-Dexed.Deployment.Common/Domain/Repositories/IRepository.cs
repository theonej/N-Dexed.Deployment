using System;
using System.Collections.Generic;

namespace N_Dexed.Deployment.Common.Domain.Repositories
{
    public interface IRepository<IItemInfo>
    {
        IItemInfo Get(IItemInfo item);
        Guid Save(IItemInfo item);
        List<IItemInfo> Search(IItemInfo searchCriteria);
        void Delete(IItemInfo item);
    }
}

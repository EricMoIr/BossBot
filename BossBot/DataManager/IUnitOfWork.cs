using BossBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Boss> BossRepository { get; }

        void Save();
    }
}

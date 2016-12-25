using BossBot;
using DataManager.XML.Models;
using System;
using System.Configuration;

namespace DataManager.XML
{
    public class XMLUnitOfWork : IUnitOfWork
    {
        private string BOSS_PATH = ConfigurationManager.AppSettings["bossesPath"];
        private XMLRepository<Boss> bossRepository;

        public XMLUnitOfWork()
        {

        }

        public IRepository<Boss> BossRepository
        {
            get
            {
                if (bossRepository == null)
                {
                    bossRepository = new XMLRepository<Boss>(BOSS_PATH);
                }
                return bossRepository;
            }
        }

        public void Save()
        {

        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

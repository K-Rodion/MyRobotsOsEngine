using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Robots;

namespace OsEngine.MyEntity
{
    public class Emitent:BaseVM
    {
        public Emitent(Security security)
        {
            _security = security;
        }

        public string NameSec
        {
            get => _security.Name;
        }

        private Security _security;
    }
}

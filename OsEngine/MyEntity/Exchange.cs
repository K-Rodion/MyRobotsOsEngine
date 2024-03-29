﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Market;
using OsEngine.Robots;

namespace OsEngine.MyEntity
{
    public class Exchange:BaseVM
    {
        public Exchange(ServerType type)
        {
            Server = type;
        }

        public ServerType Server
        {
            get => _server;

            set
            {
                _server = value;
                OnPropertyChanged(nameof(Server));
            }
        }
        private ServerType _server;
    }
}

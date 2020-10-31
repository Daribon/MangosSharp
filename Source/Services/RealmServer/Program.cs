﻿//
// Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
//
// This program is free software. You can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation. either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY. Without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Mangos.Realm;
using RealmServer.Modules;

namespace RealmServer
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder
                .RegisterModule<LoggerModule>()
                .RegisterModule<ConfigurationModule>()
                .RegisterModule<StorageModule>()
                .RegisterModule<TcpServerModule>()
                .RegisterModule<CommonModule>()
                .RegisterModule<RealmModule>();

            var container = builder.Build();
            var startup = container.Resolve<Startup>();
            await startup.StartAsync();

            Thread.CurrentThread.Join();
        }
    }
}

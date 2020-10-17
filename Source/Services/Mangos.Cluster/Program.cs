﻿//
//  Copyright (C) 2013-2020 getMaNGOS <https:\\getmangos.eu>
//  
//  This program is free software. You can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation. either version 2 of the License, or
//  (at your option) any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY. Without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

using Autofac;
using global;
using Mangos.Cluster.DataStores;
using Mangos.Cluster.Globals;
using Mangos.Cluster.Handlers;
using Mangos.Common;
using Mangos.DataStores;
using Mangos.Loggers;
using Mangos.Loggers.Console;
using Mangos.Zip;
using System.Threading.Tasks;
using Mangos.Cluster.Factories;
using Mangos.Cluster.Handlers.Guild;
using Mangos.Cluster.Network;
using Mangos.Cluster.Stats;
using Mangos.Configuration;
using Mangos.Configuration.Store;
using Mangos.Configuration.Xml;
using Mangos.Network.Tcp;

namespace Mangos.Cluster
{
    public static class Program
    {
        public static async Task Main()
        {
            var container = CreateContainer();
            var worldCluster = container.Resolve<WorldCluster>();
            await worldCluster.StartAsync();
        }

        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            RegisterConfiguration(builder);
            RegisterLoggers(builder);

            RegisterTcpServer(builder);
            RegisterDataStoreProvider(builder);

            RegisterServices(builder);
            return builder.Build();
		}

        public static void RegisterConfiguration(ContainerBuilder builder)
        {
            builder.Register(x => new XmlFileConfigurationProvider<ClusterConfiguration>(
                    x.Resolve<ILogger>(), "configs/WorldCluster.ini"))
                .As<IConfigurationProvider<ClusterConfiguration>>()
                .SingleInstance();
            builder.RegisterDecorator<StoredConfigurationProvider<ClusterConfiguration>,
                IConfigurationProvider<ClusterConfiguration>>();
        }

        public static void RegisterLoggers(ContainerBuilder builder)
        {
            builder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();
        }

        private static void RegisterTcpServer(ContainerBuilder builder)
        {
            builder.RegisterType<TcpServer>().AsSelf().SingleInstance();
            builder.RegisterType<ClientClassFactory>().As<ITcpClientFactory>().SingleInstance();
        }

        private static void RegisterDataStoreProvider(ContainerBuilder builder)
        {
            builder.RegisterType<DataStoreProvider>().As<DataStoreProvider>().SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<MangosGlobalConstants>().As<MangosGlobalConstants>().SingleInstance();
            builder.RegisterType<Common.Globals.Functions>().As<Common.Globals.Functions>().SingleInstance();
            builder.RegisterType<Common.Functions>().As<Common.Functions>().SingleInstance();
            builder.RegisterType<ZipService>().As<ZipService>().SingleInstance();
            builder.RegisterType<NativeMethods>().As<NativeMethods>().SingleInstance();
            builder.RegisterType<WorldCluster>().As<WorldCluster>().SingleInstance();
            builder.RegisterType<WorldServerClass>().As<WorldServerClass>().SingleInstance();
            builder.RegisterType<WS_DBCDatabase>().As<WS_DBCDatabase>().SingleInstance();
            builder.RegisterType<WS_DBCLoad>().As<WS_DBCLoad>().SingleInstance();
            builder.RegisterType<Globals.Functions>().As<Globals.Functions>().SingleInstance();
            builder.RegisterType<Packets>().As<Packets>().SingleInstance();
            builder.RegisterType<WC_Guild>().As<WC_Guild>().SingleInstance();
            builder.RegisterType<WC_Stats>().As<WC_Stats>().SingleInstance();
            builder.RegisterType<WC_Network>().As<WC_Network>().SingleInstance();
            builder.RegisterType<WC_Handlers>().As<WC_Handlers>().SingleInstance();
            builder.RegisterType<WC_Handlers_Auth>().As<WC_Handlers_Auth>().SingleInstance();
            builder.RegisterType<WC_Handlers_Battleground>().As<WC_Handlers_Battleground>().SingleInstance();
            builder.RegisterType<WC_Handlers_Chat>().As<WC_Handlers_Chat>().SingleInstance();
            builder.RegisterType<WC_Handlers_Group>().As<WC_Handlers_Group>().SingleInstance();
            builder.RegisterType<WC_Handlers_Guild>().As<WC_Handlers_Guild>().SingleInstance();
            builder.RegisterType<WC_Handlers_Misc>().As<WC_Handlers_Misc>().SingleInstance();
            builder.RegisterType<WC_Handlers_Movement>().As<WC_Handlers_Movement>().SingleInstance();
            builder.RegisterType<WC_Handlers_Social>().As<WC_Handlers_Social>().SingleInstance();
            builder.RegisterType<WC_Handlers_Tickets>().As<WC_Handlers_Tickets>().SingleInstance();
            builder.RegisterType<WS_Handler_Channels>().As<WS_Handler_Channels>().SingleInstance();
            builder.RegisterType<WcHandlerCharacter>().As<WcHandlerCharacter>().SingleInstance();

            builder.RegisterType<ClusterServiceLocator>().As<ClusterServiceLocator>()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .SingleInstance();
        }
    }
}
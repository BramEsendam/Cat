﻿using Cat.Discord;
using Cat.Discord.Configurations;
using Cat.Discord.Handlers;
using Cat.Discord.Interfaces;
using Cat.Discord.Services;
using Cat.Discord.Services.Implementations;
using Cat.Interfaces;
using Cat.Persistence.EntityFrameworkCore.Models;
using Cat.Persistence.EntityFrameworkCore.Repositories;
using Cat.Persistence.EntityFrameworkCore.UnitOfWork;
using Cat.Persistence.Interfaces.Repositories;
using Cat.Persistence.Interfaces.UnitOfWork;
using Cat.Services;
using Cat.Services.Implementations;
using Discord.WebSocket;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Cat
{
    public static class Unity
    {
        private static UnityContainer container;

        private static UnityContainer Container
        {
            get
            {
                if (container == null)
                    RegisterTypes();
                return container;
            }
        }
        public static void RegisterTypes()
        {
            container = new UnityContainer();
            container.RegisterType<ILogger, Logger>(new PerThreadLifetimeManager());

            container.RegisterFactory<DiscordSocketConfig>(i => SocketConfig.GetDefault(), new SingletonLifetimeManager());
            container.RegisterSingleton<DiscordShardedClient>(new InjectionConstructor(typeof(DiscordSocketConfig)));

            container.RegisterSingleton<IConnection, Connection>();
            container.RegisterSingleton<ICommandHandler, CommandHandler>();
            container.RegisterSingleton<IExpService, ExpService>();
            container.RegisterSingleton<IExpHandler, ExpHandler>();
            container.RegisterSingleton<IUserLeftHandler, UserLeftHandler>();
            container.RegisterSingleton<EventMessageHandler>();
            container.RegisterSingleton<ICat, Cat>();

            container.RegisterType<CatContext>(new PerResolveLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerResolveLifetimeManager());
            container.RegisterType<IServerRepository, ServerRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IUserRepository, UserRepository>(new PerResolveLifetimeManager());
            container.RegisterType<ILogsRepository, LogsRepository>(new PerResolveLifetimeManager());

            container.RegisterType<IDiscordLogger, DiscordLogger>(new PerThreadLifetimeManager());
        }

        public static T Resolve<T>()
        {
            return (T)Container.Resolve(typeof(T));
        }
    }
}

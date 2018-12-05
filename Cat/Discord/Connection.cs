﻿using System;
using System.Threading.Tasks;
using Cat.Configurations;
using Cat.Discord.Interfaces;
using Cat.Discord.Services;
using Cat.Persistence.Interfaces.UnitOfWork;
using Cat.Persistence.JsonStorage;
using Cat.Services;
using Discord;
using Discord.WebSocket;
using IConnection = Cat.Discord.Interfaces.IConnection;

namespace Cat.Discord
{
    public class Connection : IConnection
    {
        private readonly DiscordShardedClient _client;
        private readonly IDiscordLogger _discordLogger;
        private readonly ILogger _logger;
        private readonly ICommandHandler _commandHandler;
        private readonly IExpHandler _expHandler;

        public Connection(DiscordShardedClient client, IDiscordLogger discordLogger, ILogger logger, ICommandHandler commandHandler, IExpHandler expHandler)
        {
            _client = client;
            _discordLogger = discordLogger;
            _logger = logger;
            _commandHandler = commandHandler;
            _expHandler = expHandler;
        }
        
        public async Task ConnectAsync()
        {
            _client.Log += _discordLogger.Log;
            _client.ShardLatencyUpdated += ShardLatencyUpdatedAsync;
            _client.ShardDisconnected += ShardDisconnectedAsync;
            //_client.ShardConnected += _client_ShardConnected;

            await _client.LoginAsync(TokenType.Bot, ConfigData.Data.Token).ConfigureAwait(false);
            await _client.StartAsync().ConfigureAwait(false);
            await _commandHandler.InitializeAsync(_client).ConfigureAwait(false);
            _expHandler.Initialize(_client);

            await Task.Delay(ConfigData.Data.RestartTime * 60000).ConfigureAwait(false);
            await _client.StopAsync().ConfigureAwait(false);
        }

        private async Task _client_ShardConnected(DiscordSocketClient client)
        {
            var dataLoader = new DataLoader();
            await dataLoader.SaveDataToDatabase(client, Unity.Resolve<IUnitOfWork>()).ConfigureAwait(false);
        }

        private Task ShardDisconnectedAsync(Exception exception, DiscordSocketClient shard)
        {
            _logger.Log("Connection/Disconnected", $"Shard: {shard.ShardId} reason: {exception.Message}");
            return Task.CompletedTask;
        }

        private Task ShardLatencyUpdatedAsync(int oldPing, int updatePing, DiscordSocketClient shard)
        {
            if (updatePing >= 500) _logger.Log("Connection/Latency", $"Shard: {shard.ShardId} Latency: {updatePing}");
            return Task.CompletedTask;
        }
    }
}

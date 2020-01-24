using System;
using System.Threading;
using System.Threading.Tasks;
using RockPaperScissors.Interfaces;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Runtime;

using System.Collections.Generic;
using System.Linq;

namespace OrleansClient
{
    public class RockPaperScissorsClientHostedService : IHostedService
    {
        readonly Dictionary<char, Weapon> weaponCommands = new Dictionary<char, Weapon>()
        {
            { 'r', Weapon.Rock },
            { 'p', Weapon.Paper },
            { 's', Weapon.Scissors },
        };

        private readonly IClusterClient _client;

        public RockPaperScissorsClientHostedService(IClusterClient client)
        {
            this._client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var player = this._client.GetGrain<IPlayer>(Guid.NewGuid());
            var match = await player.FindMatch();
            var id = match.GetPrimaryKey();
            Console.WriteLine($"Joined match {id}");
            var run = true;
            while (run)
            {
                Console.WriteLine($"[r]ock, [p]aper, [s]cissors?");
                var input = Console.ReadKey().KeyChar;
                if (input == 'q')
                {
                    run = false;
                }
                else
                {
                    var success = weaponCommands.TryGetValue(input, out Weapon attackWeapon);
                    Console.Write("\b");
                    if (success)
                    {
                        Console.WriteLine($"{attackWeapon.ToString()} Attack!");
                        var result = await player.Attack(attackWeapon);
                        Console.WriteLine($"{result}");
                    }
                    else
                    {
                        Console.WriteLine("Not ready yet!");
                    }
                }
            }
            this._client.Dispose();
            Environment.Exit(0);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

using RockPaperScissors.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using System;
using Orleans;

namespace RockPaperScissors.Grains
{
    public class MatchGrain : Orleans.Grain, IMatch
    {
        private readonly ILogger logger;

        private IPlayer player1;
        public Task<IPlayer> Player1() => Task.FromResult(player1);
        private IPlayer player2;
        public Task<IPlayer> Player2() => Task.FromResult(player2);

        public MatchGrain(ILogger<MatchGrain> logger)
        {
            this.logger = logger;
        }
        public Task<bool> AddPlayer(IPlayer player)
        {
            if (player1 == null)
            {
                player1 = player;
            }
            else if (player2 == null)
            {
                player2 = player;
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}

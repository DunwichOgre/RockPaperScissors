using RockPaperScissors.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using System;
using Orleans;

namespace RockPaperScissors.Grains
{
    public class PlayerGrain : Orleans.Grain, IPlayer
    {
        private readonly ILogger logger;

        private IMatch currentMatch;
        private Nullable<Weapon> attackWeapon;
        public Task<Nullable<Weapon>> GetAttackWeapon() => Task.FromResult(attackWeapon);

        public PlayerGrain(ILogger<PlayerGrain> logger)
        {
            this.logger = logger;
        }
        public async Task<IMatch> FindMatch()
        {
            var found = false;
            long grainId = 0;
            while (!found)
            {
                currentMatch = this.GrainFactory.GetGrain<IMatch>(grainId);
                found = await currentMatch.AddPlayer(this);
                grainId++;
            }
            logger.LogInformation($"Player joined game {grainId}");

            return currentMatch;
        }
        public async Task<string> Attack(Weapon weapon)
        {
            var weaponDescription = weapon.ToString();
            logger.LogInformation($"Attack message received: weapon [{weaponDescription}]");
            this.attackWeapon = weapon;
            var opponent = await FindOpponent();
            if (opponent == null)
            {
                return $"No opponent connected yet!\nYou will attack with [{weaponDescription}] when they connect.";
            }
            var opponentWeapon = await opponent.GetAttackWeapon();
            if (opponentWeapon == null)
            {
                return $"Opponent has not attacked yet!\nYou will attack with [{weaponDescription}] when they attack.";
            }
            var opponentWeaponDescription = opponentWeapon.ToString();
            var opponentResult = $"Attacked opponent with [{opponentWeaponDescription}]!\nOpponent attacked with [{weaponDescription}].";
            await opponent.SendMessage(opponentResult);
            var result = $"Attacking opponent with [{weaponDescription}]!\nOpponent attacking with [{opponentWeaponDescription}].";
            return result;
        }
        private async Task<IPlayer> FindOpponent()
        {
            var player1 = await currentMatch.Player1();
            var player2 = await currentMatch.Player2();
            if (player1?.GetGrainIdentity() == this.GetGrainIdentity() && player2 != null) return player2;
            if (player2?.GetGrainIdentity() == this.GetGrainIdentity() && player1 != null) return player1;
            return null;
        }
        public Task SendMessage(string message)
        {
            // TODO: Breakpoint is hit and message shows up on host, suspect we need Orleans Observers or some type of Event to force the opponent's Grain to render the result of the battle
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}

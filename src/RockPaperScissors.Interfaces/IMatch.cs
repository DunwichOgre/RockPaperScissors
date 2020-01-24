using System.Threading.Tasks;

using System;

namespace RockPaperScissors.Interfaces
{
    public interface IMatch : Orleans.IGrainWithIntegerKey
    {
        Task<IPlayer> Player1();
        Task<IPlayer> Player2();
        Task<bool> AddPlayer(IPlayer player);
    }
}

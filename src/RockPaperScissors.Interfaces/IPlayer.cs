using System.Threading.Tasks;

using System;

namespace RockPaperScissors.Interfaces
{
    public enum Weapon
    {
        Rock,
        Paper,
        Scissors
    }
    public interface IPlayer : Orleans.IGrainWithGuidKey
    {
        Task<IMatch> FindMatch();
        Task<string> Attack(Weapon weapon);
        Task<Nullable<Weapon>> GetAttackWeapon();
        Task SendMessage(string message);
    }
}

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleDataModel
{
    public abstract class AiPlayerStrategyBase
    {
        public abstract void PlayNextMove(Battle battle, Player player);

        /// <summary>
        /// Returns the odds of winning an attack. When the attacker has fewer dice, 0 is returned.
        /// When the attacker has at least two more dice, 1 is returned.
        /// Otherwise, returns the percentage chance of the attacker winning (according to https://anydice.com/program/3af90)
        /// </summary>
        protected float GetNormalizedChanceOfWinningAttack(MapNode attacker, MapNode defender)
        {
            if (attacker.NumDice < defender.NumDice)
            {
                return 0;
            }
            
            if (attacker.NumDice > defender.NumDice + 1)
            {
                return 1f;
            }

            return (attacker.NumDice, defender.NumDice) switch
            {
                (1,1) => 0, // attacking with one die is not permitted
                (2,2) => 44.3672839506f,
                (3,3) => 45.3575102881f,
                (4,4) => 45.9528249314f,
                (5,5) => 46.3653597013f,
                (6,6) => 46.6730601952f,
                (7,7) => 46.913916814f,
                (8,8) => 47.1090727064f,
                
                (2,1) => 83.7962962963f,
                (3,2) => 77.8549382716f,
                (4,3) => 74.2830504115f,
                (5,4) => 71.8078417924f,
                (6,5) => 69.9616388287f,
                (7,6) => 68.5164991159f,
                (8,7) => 67.3455637616f,

                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
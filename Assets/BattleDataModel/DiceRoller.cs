using System;
using System.Collections.Generic;

namespace BattleDataModel
{
    public static class DiceRoller
    {
        public static List<int> RollDice(int numDice, Random rng)
        {
            List<int> rollResults = new List<int>();

            for (int i = 0; i < numDice; i++)
            {
                rollResults.Add(rng.Next(1, 7));
            }

            return rollResults;
        }
    }
}
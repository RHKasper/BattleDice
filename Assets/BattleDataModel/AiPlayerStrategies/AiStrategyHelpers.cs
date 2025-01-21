using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleDataModel.AiPlayerStrategies
{
    public static class AiStrategyHelpers
    {
        /// <summary>
        /// Returns the odds of winning an attack. When the attacker has fewer dice, 0 is returned.
        /// When the attacker has at least two more dice, 1 is returned.
        /// Otherwise, returns the percentage chance of the attacker winning (according to https://anydice.com/program/3af90)
        /// </summary>
        public static float GetNormalizedChanceOfWinningAttack(MapNode attacker, MapNode defender)
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
        
        public static bool CapturingTerritoryWouldGrowLargestRegion(MapNode attackingTerritory, MapNode defendingTerritory, HashSet<MapNode> myReinforcementTerritories)
        {
            bool attackingTerritoryIsInLargestRegion = myReinforcementTerritories.Contains(attackingTerritory);
            bool defendingTerritoryHasAlliedAdjacentsNotInLargestRegion = defendingTerritory.AdjacentNodes.Any(t =>
                t.OwnerPlayerIndex == attackingTerritory.OwnerPlayerIndex && !myReinforcementTerritories.Contains(t));
            
            bool defendingTerritoryHasAlliedAdjacentsInLargestRegion =  defendingTerritory.AdjacentNodes.Any(t =>
                t.OwnerPlayerIndex == attackingTerritory.OwnerPlayerIndex && myReinforcementTerritories.Contains(t));
            
            return attackingTerritoryIsInLargestRegion ? defendingTerritoryHasAlliedAdjacentsNotInLargestRegion : defendingTerritoryHasAlliedAdjacentsInLargestRegion;
        }

        public static bool IsStartOfAnAttackChain(MapNode attackingTerritory, MapNode defendingTerritory, out int attackChainLength)
        {
            // todo: extend this logic to look for longer attackable chains
            
            bool yes = defendingTerritory.AdjacentNodes.Any(t => t.OwnerPlayerIndex != attackingTerritory.OwnerPlayerIndex && t.NumDice < defendingTerritory.NumDice);
            attackChainLength = yes ? 2 : 1;
            return yes;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace BattleDataModel.AiPlayerStrategies
{
    public class DefensiveAiStrategy : AiPlayerStrategyBase 
    {
        protected override float GetDesirabilityScoreForPotentialAttack(MapNode attackingTerritory, MapNode defendingTerritory, Battle battle, HashSet<MapNode> myReinforcementTerritories)
        {
            float score = 1;
            Dictionary<int, int> totalDiceForEachPlayer = GetTotalDiceForEachPlayer(battle);
            bool thirtyPercentMoreDice = totalDiceForEachPlayer.All(p => p.Key == attackingTerritory.OwnerPlayerIndex || p.Value * 1.3f < totalDiceForEachPlayer[attackingTerritory.OwnerPlayerIndex]);
            bool fiftyPercentMoreDice = totalDiceForEachPlayer.All(p => p.Key == attackingTerritory.OwnerPlayerIndex || p.Value * 1.5f < totalDiceForEachPlayer[attackingTerritory.OwnerPlayerIndex]);
            
            
            
            // if target territory is part of an enemy's largest region, rate it higher
            if (battle.Map.GetLargestContiguousGroupOfTerritories(defendingTerritory.OwnerPlayerIndex).Contains(defendingTerritory))
            {
                score++;
            }
            
            // if owning target territory would add 2+ territories to the attacker's largest region, rate it higher
            if (AiStrategyHelpers.CapturingTerritoryWouldGrowLargestRegion(attackingTerritory, defendingTerritory, myReinforcementTerritories))
            {
                score += 2;
            }
            
            // attack conservatively based on how dominant this player is in num dice
            if (fiftyPercentMoreDice)
            {
                if (attackingTerritory.NumDice > defendingTerritory.NumDice)
                {
                    score += 2;
                }
                else if (attackingTerritory.NumDice == defendingTerritory.NumDice)
                {
                    score++;
                }
                else
                {
                    score += .25f;
                }
            }
            else if (thirtyPercentMoreDice)
            {
                if (attackingTerritory.NumDice > defendingTerritory.NumDice + 1)
                {
                    score += 2;
                }
                else if (attackingTerritory.NumDice > defendingTerritory.NumDice)
                {
                    score++;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (AiStrategyHelpers.GetNormalizedChanceOfWinningAttack(attackingTerritory.NumDice, defendingTerritory.NumDice) > .7f)
                {
                    score++;
                }
                else
                {
                    return 0;
                }
            }
            
            float chanceOfSuccess = AiStrategyHelpers.GetNormalizedChanceOfWinningAttack(attackingTerritory, defendingTerritory);

            return score * chanceOfSuccess;
        }

        private static Dictionary<int, int> GetTotalDiceForEachPlayer(Battle battle)
        {
            Dictionary<int, int> playerTotalDice = new Dictionary<int, int>();
            
            foreach (Player player in battle.Players)
            {
                playerTotalDice[player.PlayerIndex] = 0;
            }

            foreach (var territory in battle.Map.Territories.Values)
            {
                playerTotalDice[territory.OwnerPlayerIndex] += territory.NumDice;
            }

            return playerTotalDice;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleDataModel.AiPlayerStrategies
{
    public class AggressiveAiStrategy : AiPlayerStrategyBase
    {
        /// <summary>
        /// Calculates a score for how desirable a particular attack would be. Values less than or equal to 0 are a "no".
        /// The highest scoring attack above 0 will be chosen 
        /// </summary>
        protected override float GetDesirabilityScoreForPotentialAttack(MapNode attackingTerritory, MapNode defendingTerritory, Battle battle, HashSet<MapNode> myReinforcementTerritories)
        {
            float score = 1;

            // if target territory is part of an attack chain, rate it higher
            if (AiStrategyHelpers.IsStartOfAnAttackChain(attackingTerritory, defendingTerritory, out int chainLength))
            {
                score += chainLength - 1;
            }
            
            // if target territory is part of an enemy's largest region, rate it higher
            if (battle.Map.GetLargestContiguousGroupOfTerritories(defendingTerritory.OwnerPlayerIndex).Contains(defendingTerritory))
            {
                score++;
            }
            
            // if owning target territory would add 2+ territories to the attacker's largest region, rate it higher
            if (AiStrategyHelpers.CapturingTerritoryWouldGrowLargestRegion(attackingTerritory, defendingTerritory, myReinforcementTerritories))
            {
                score++;
            }
            
            float chanceOfSuccess = AiStrategyHelpers.GetNormalizedChanceOfWinningAttack(attackingTerritory, defendingTerritory);
            
            if (chanceOfSuccess < .46f)
            {
                return 0;
            }
            
            return score * chanceOfSuccess;
        }
    }
}
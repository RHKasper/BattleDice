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

            // if target territory is part of a longer attack chain, rate it higher
            if (AiStrategyHelpers.IsStartOfAnAttackChain(attackingTerritory, defendingTerritory, out int chainLength))
            {
                score += chainLength;
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
            
            // only attack with even dice when the stack is 5 or greater
            if (attackingTerritory.NumDice == defendingTerritory.NumDice && attackingTerritory.NumDice < 5)
            {
                return 0;
            }
            
            return score * chanceOfSuccess;
        }
    }
}
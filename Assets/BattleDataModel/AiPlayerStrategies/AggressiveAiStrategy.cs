using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleDataModel.AiPlayerStrategies
{
    public class AggressiveAiStrategy : AiPlayerStrategyBase
    {
        public override void PlayNextMove(Battle battle, Player player)
        {
            List<MapNode> myTerritories = battle.Map.GetTerritoriesOwnedByPlayer(player.PlayerIndex);
            HashSet<MapNode> myReinforcementTerritories = battle.Map.GetLargestContiguousGroupOfTerritories(player.PlayerIndex);
            PotentialAttack bestAttackOption = null;

            foreach (MapNode territory in myTerritories)
            {
                IEnumerable<MapNode> attackCandidates = territory.AdjacentNodes.Where(t => t.OwnerPlayerIndex != player.PlayerIndex);
                foreach (MapNode attackCandidate in attackCandidates)
                {
                    var score = GetDesirabilityScoreForPotentialAttack(territory, attackCandidate, battle, myReinforcementTerritories);

                    if (score > 0)
                    {
                        if (bestAttackOption == null || score > bestAttackOption.DesirabilityScore)
                        {
                            bestAttackOption = new PotentialAttack(territory, attackCandidate, score);
                        }
                    }
                }
            }

            if (bestAttackOption != null)
            {
                battle.Attack(bestAttackOption.Attacker, bestAttackOption.Defender);
            }
            else
            {
                battle.EndTurn();
            }
        }

        /// <summary>
        /// Calculates a score for how desirable a particular attack would be. Values less than or equal to 0 are a "no".
        /// The highest scoring attack above 0 will be chosen 
        /// </summary>
        public float GetDesirabilityScoreForPotentialAttack(MapNode attackingTerritory, MapNode defendingTerritory, Battle battle, HashSet<MapNode> myReinforcementTerritories)
        {
            float score = 1;

            // if target territory has an adjacent territory that would be attackable next, rate it higher
            // todo: extend this logic to look for longer attackable chains
            if (defendingTerritory.AdjacentNodes.Any(t => t.OwnerPlayerIndex != attackingTerritory.OwnerPlayerIndex && t.NumDice < defendingTerritory.NumDice))
            {
                score++;
            }
            
            // if target territory is part of an enemy's largest region, rate it higher
            if (battle.Map.GetLargestContiguousGroupOfTerritories(defendingTerritory.OwnerPlayerIndex).Contains(defendingTerritory))
            {
                score++;
            }
            
            // if owning target territory would add 2+ territories to the attacker's largest region, rate it higher
            if (myReinforcementTerritories.Contains(attackingTerritory) && defendingTerritory.AdjacentNodes.Any(t => t.OwnerPlayerIndex == attackingTerritory.OwnerPlayerIndex && !myReinforcementTerritories.Contains(t)))
            {
                score++;
            }
            
            // todo: fancy graph logic to check if taking this node would separate an opponent's largest region
            // todo: fancy graph logic to check if taking this node would connect multiple territories belonging to this player

            float chanceOfSuccess = GetNormalizedChanceOfWinningAttack(attackingTerritory, defendingTerritory);
            
            return score * chanceOfSuccess;
        }
        
        private class PotentialAttack
        {
            public MapNode Attacker;
            public MapNode Defender;
            public float DesirabilityScore;

            public PotentialAttack(MapNode attacker, MapNode defender, float desirabilityScore)
            {
                Attacker = attacker;
                Defender = defender;
                DesirabilityScore = desirabilityScore;
            }
        }
    }
}
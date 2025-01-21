using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleDataModel
{
    public abstract class AiPlayerStrategyBase
    {
        public void PlayNextMove(Battle battle, Player player)
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
        
        protected abstract float GetDesirabilityScoreForPotentialAttack(MapNode attackingTerritory, MapNode defendingTerritory, Battle battle, HashSet<MapNode> myReinforcementTerritories);
        
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
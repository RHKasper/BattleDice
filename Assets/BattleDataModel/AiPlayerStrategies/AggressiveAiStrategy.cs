using System.Collections.Generic;
using System.Linq;

namespace BattleDataModel.AiPlayerStrategies
{
    public class AggressiveAiStrategy : AiPlayerStrategyBase
    {
        public override void PlayNextMove(Battle battle, Player player)
        {
            List<MapNode> myTerritories = battle.Map.GetTerritoriesOwnedByPlayer(player.PlayerIndex);

            foreach (MapNode territory in myTerritories)
            {
                if (territory.NumDice == 1)
                {
                    continue;
                }
                
                IEnumerable<MapNode> attackCandidates = territory.AdjacentNodes.Where(t => t.OwnerPlayerIndex != player.PlayerIndex && t.NumDice < territory.NumDice);
                MapNode bestAttackCandidate = null;
                
                foreach (MapNode attackCandidate in attackCandidates)
                {
                    if (bestAttackCandidate == null || attackCandidate.NumDice > bestAttackCandidate.NumDice)
                    {
                        bestAttackCandidate = attackCandidate;
                    }
                }

                if (bestAttackCandidate != null)
                {
                    battle.Attack(territory, bestAttackCandidate);
                    return;
                }
            }

            battle.EndTurn();
        }
    }
}
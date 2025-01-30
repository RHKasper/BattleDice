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
        public static float GetNormalizedChanceOfWinningAttack(int attackerDice, int defenderDice)
        {
            if (attackerDice < defenderDice)
            {
                return 0;
            }
            
            if (attackerDice > defenderDice + 1)
            {
                return 1f;
            }

            return (attackerDice, defenderDice) switch
            {
                (1,1) => 0, // attacking with one die is not permitted
                (2,2) => .443672839506f,
                (3,3) => .453575102881f,
                (4,4) => .459528249314f,
                (5,5) => .463653597013f,
                (6,6) => .466730601952f,
                (7,7) => .46913916814f,
                (8,8) => .471090727064f,
                
                (2,1) => .837962962963f,
                (3,2) => .778549382716f,
                (4,3) => .742830504115f,
                (5,4) => .718078417924f,
                (6,5) => .699616388287f,
                (7,6) => .685164991159f,
                (8,7) => .673455637616f,

                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        
        /// <summary>
        /// Returns the odds of winning an attack. When the attacker has fewer dice, 0 is returned.
        /// When the attacker has at least two more dice, 1 is returned.
        /// Otherwise, returns the percentage chance of the attacker winning (according to https://anydice.com/program/3af90)
        /// </summary>
        public static float GetNormalizedChanceOfWinningAttack(MapNode attacker, MapNode defender)
        {
            return GetNormalizedChanceOfWinningAttack(attacker.NumDice, defender.NumDice);
        }
        
        public static bool CapturingTerritoryWouldGrowLargestRegionByTwoOrMore(MapNode defendingTerritory, HashSet<MapNode> myLargestRegion)
        {
            int playerIndex = myLargestRegion.First().OwnerPlayerIndex;
            bool adjacentTerritoryIsInMyLargestRegion = defendingTerritory.AdjacentNodes.Any(myLargestRegion.Contains);
            bool adjacentAlliedTerritoryIsNotInMyLargestRegion = defendingTerritory.AdjacentNodes.Any( t=> t.OwnerPlayerIndex == playerIndex && !myLargestRegion.Contains(t));
            return adjacentTerritoryIsInMyLargestRegion && adjacentAlliedTerritoryIsNotInMyLargestRegion;
        }

        public static bool CanReachWeakTerritoryOfTargetPlayer(MapNode attackerTerritory, int targetPlayerID)
        {
            var chains = BreadthFirstTraversal(new AttackChain(attackerTerritory), (chain, node) =>
            {
                bool isEnemy = node.OwnerPlayerIndex != attackerTerritory.OwnerPlayerIndex;
                bool isWeak = node.NumDice < attackerTerritory.NumDice - chain.NumberOfAttacks;
                return isEnemy && isWeak;
            });

            return chains.Any(c => c.Territories.Any(t => t.OwnerPlayerIndex == targetPlayerID));
        }

        public static int GetAttackChainLength(MapNode attackingTerritory, MapNode defendingTerritory)
        {
            bool isWeak = defendingTerritory.NumDice < attackingTerritory.NumDice;

            if (!isWeak)
            {
                return 0;
            }
            
            var attackChains = BreadthFirstTraversal(new AttackChain(attackingTerritory, defendingTerritory), (chain, mapNode) =>
            {
                bool isNotYetPartOfChain = chain.Territories.Contains(mapNode) == false;
                bool isEnemy = mapNode.OwnerPlayerIndex != attackingTerritory.OwnerPlayerIndex;
                bool isWeak = mapNode.NumDice < attackingTerritory.NumDice - chain.NumberOfAttacks;
                return isNotYetPartOfChain && isEnemy && isWeak;
            });

            return attackChains.Last().NumberOfAttacks;
        }

        private static List<AttackChain> BreadthFirstTraversal(AttackChain startingChain, Func<AttackChain, MapNode, bool> acceptanceCriteria, Func<List<AttackChain>, bool> endCondition = null)
        {
            List<AttackChain> chains = new List<AttackChain> {startingChain};

            for (int i = 0; i < chains.Count; i++)
            {
                AttackChain chain = chains[i];
                MapNode currentEndOfChain = chain.Territories.Last.Value;
                
                foreach (MapNode mapNode in currentEndOfChain.AdjacentNodes)
                {
                    if(acceptanceCriteria.Invoke(chain, mapNode))
                    {
                        chains.Add(new AttackChain(chain, mapNode));
                    }
                }

                if (endCondition != null &&  endCondition.Invoke(chains))
                {
                    break;
                }
            }

            return chains;
        }

        public static AiPlayerStrategyBase GetAiStrategyObject(AiStrat strat)
        {
            return strat switch
            {
                AiStrat.Aggressive => new AggressiveAiStrategy(),
                AiStrat.Defensive => new DefensiveAiStrategy(),
                _ => throw new ArgumentOutOfRangeException(nameof(strat), strat, null)
            };
        }

        private class AttackChain
        {
            public LinkedList<MapNode> Territories { get; }

            public int NumberOfAttacks => Territories.Count - 1;

            public AttackChain(MapNode attackingTerritory)
            {
                Territories = new LinkedList<MapNode>();
                Territories.AddFirst(attackingTerritory);
            }
            
            public AttackChain(MapNode attackingTerritory, MapNode defendingTerritory)
            {
                Territories = new LinkedList<MapNode>();
                Territories.AddFirst(attackingTerritory);
                Territories.AddLast(defendingTerritory);
            }

            public AttackChain(AttackChain baseChain, MapNode endOfNewChain)
            {
                Territories = new LinkedList<MapNode>();

                foreach (var territory in baseChain.Territories)
                {
                    Territories.AddLast(territory);
                }

                Territories.AddLast(endOfNewChain);
            }
        }
    }
}
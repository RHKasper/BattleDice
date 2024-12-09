using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace BattleDataModel
{
    public class Battle
    {
        public event EventHandler<BattleEvents.StartingTerritoriesAssignedArgs> StartingTerritoriesAssigned;
        public event EventHandler<BattleEvents.StartingReinforcementsAllocatedArgs> StartingReinforcementsAllocated;
        public event EventHandler<BattleEvents.TerritoryCapturedArgs> TerritoryCaptured;
        
        private readonly List<Player> _players;
        private int _activePlayerIndex = 0;
        
        public Map Map { get; }
        internal Random Rng { get; }
        public Player ActivePlayer => _players[_activePlayerIndex];
        public IReadOnlyList<Player> Players => _players;


        public Battle(Map map, List<Player> players, int randomSeed)
        {
            _players = players.ToList();
            Map = map;
            Rng = new Random(randomSeed);
        }

        /// <summary>
        /// Randomly assigns each <see cref="MapNode"/> of <see cref="Map"/> to <see cref="_players"/>, ensuring they're
        /// spread as evenly as possible, and sets each node's <see cref="MapNode.NumDice"/> to 1
        /// </summary>
        public void RandomlyAssignTerritories()
        {
            // todo: optimize all this to avoid so much memory allocation and inefficient CPU usage
            
            List<MapNode> shuffledNodes = Map.Nodes.Values.OrderBy(_ => Rng.Next()).ToList();
            List<Player> shuffledPlayers = _players.ToList();
            
            for (var i = 0; i < shuffledNodes.Count; i++)
            {
                var playerIndex = i % _players.Count;
                if (playerIndex == 0)
                {
                    // reshuffle
                    shuffledPlayers = shuffledPlayers.OrderBy(_ => Rng.Next()).ToList();
                }

                shuffledNodes[i].OwnerPlayerId = shuffledPlayers[playerIndex].PlayerID;
            }
            
            StartingTerritoriesAssigned?.Invoke(this, new BattleEvents.StartingTerritoriesAssignedArgs(this));
        }

        public void RandomlyAllocateStartingReinforcements(int startingReinforcements)
        {
            if (Map.Nodes.Values.Any(n => n.OwnerPlayerId == -1))
            {
                throw new Exception("All map territories (nodes) must belong to a player before starting reinforcements are applied");
            }

            Dictionary<int, List<MapNode>> playerNodes = new();

            foreach (Player player in _players)
            {
                playerNodes[player.PlayerID] = new();
            }

            foreach (var mapNode in Map.Nodes.Values)
            {
                mapNode.NumDice = 1;
                playerNodes[mapNode.OwnerPlayerId].Add(mapNode);
            }

            foreach (Player player in _players)
            {
                var nodes = playerNodes[player.PlayerID];
                
                for (int i = 0; i < startingReinforcements; i++)
                {
                    playerNodes[player.PlayerID][Rng.Next(0, nodes.Count)].NumDice++;
                    // todo: trigger dice changed event?
                }
            }
            
            StartingReinforcementsAllocated?.Invoke(this, new BattleEvents.StartingReinforcementsAllocatedArgs(this));
        }

        public void Attack(MapNode attackingSpace, MapNode defendingSpace)
        {
            Debug.Log("node " + attackingSpace.NodeId + " attacks node " + defendingSpace.NodeId);
            
            List<int> attackingRoll = DiceRoller.RollDice(attackingSpace.NumDice, Rng);
            List<int> defendingRoll = DiceRoller.RollDice(defendingSpace.NumDice, Rng);
            int attackRollSum = attackingRoll.Sum();
            int defenseRollSum = defendingRoll.Sum();

            bool attackerWins = attackRollSum > defenseRollSum;
            string resultsString = attackRollSum + " vs " + defenseRollSum + " ([" + string.Join(", ", attackingRoll) + "] vs [" + string.Join(", ", defendingRoll) + "]";

            if (attackerWins)
            {
                Debug.Log("Attacker wins: " + resultsString);
                defendingSpace.NumDice = attackingSpace.NumDice - 1;
                attackingSpace.NumDice = 1;
                ChangeTerritoryOwnership(defendingSpace, attackingSpace.OwnerPlayerId);
            }
            else
            {
                Debug.Log("Defender wins: " + resultsString);
                attackingSpace.NumDice = 1;
            }
        }

        public void EndTurn()
        {
            Reinforce(_activePlayerIndex);
            _activePlayerIndex = (_activePlayerIndex + 1) % Players.Count;
        }
        
        /// <summary>
        /// Called at the end of a turn, this method randomly distributes a number of reinforcement dice equal to
        /// the size of the largest contiguous number of nodes owned by the given <see cref="playerId"/>
        /// </summary>
        private void Reinforce(int playerId)
        {
            HashSet<MapNode> largestContiguousTerritory = Map.GetLargestContiguousTerritory(playerId);
            int reinforcementsCount = largestContiguousTerritory.Count;
            List<MapNode> ownedNodes = Map.GetNodesOwnedByPlayer(playerId);
            List<MapNode> ownedNodesWithRoomForReinforcements = ownedNodes.Where(n => n.NumDice < Constants.MaxDiceInTerritory).ToList();

            Debug.Log("Adding " + reinforcementsCount + " reinforcements for player " + playerId);
            
            for (int i = 0; i < reinforcementsCount; i++)
            {
                if (ownedNodesWithRoomForReinforcements.Count == 0)
                {
                    HandleNoRoomForReinforcements(reinforcementsCount - i);
                    break;    
                }
                
                int randomTerritoryIndex = Rng.Next(0, ownedNodesWithRoomForReinforcements.Count);
                MapNode randomTerritory = ownedNodesWithRoomForReinforcements[randomTerritoryIndex];
                randomTerritory.NumDice++;
                
                if (randomTerritory.NumDice == Constants.MaxDiceInTerritory)
                {
                    ownedNodesWithRoomForReinforcements.RemoveAt(randomTerritoryIndex);
                }
            }
        }

        private void HandleNoRoomForReinforcements(int extraReinforcementsAmount)
        {
            Debug.Log("No room for " + extraReinforcementsAmount + " extra reinforcements");
        }

        private void ChangeTerritoryOwnership(MapNode territory, int newOwnerPlayerId)
        {
            var eventArgs = new BattleEvents.TerritoryCapturedArgs(territory, territory.OwnerPlayerId);
            territory.OwnerPlayerId = newOwnerPlayerId;
            // todo: handle player elimination and end game
            TerritoryCaptured?.Invoke(this, eventArgs);
        }
    }
}
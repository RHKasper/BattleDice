using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace BattleDataModel
{
    public class Battle
    {
        private readonly List<Player> _players;
        private int _activePlayerIndex = 0;

        public Player ActivePlayer => _players[_activePlayerIndex];
        public Map Map { get; }
        public IReadOnlyList<Player> Players => _players;
        internal Random Rng { get; private set; }

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
        }

        public void Attack(int attackingSpaceId, int defendingSpaceId)
        {
            
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
    }
}
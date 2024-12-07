using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace BattleDataModel
{
    public class Battle
    {
        private readonly List<Player> _players;

        public int ActivePlayerIndex { get; private set; }
        public Map Map { get; }
        public IReadOnlyList<Player> Players => _players;

        public Battle(Map map, List<Player> players)
        {
            _players = players.ToList();
            Map = map;
        }

        /// <summary>
        /// Randomly assigns each <see cref="MapNode"/> of <see cref="Map"/> to <see cref="_players"/>, ensuring they're
        /// spread as evenly as possible, and sets each node's <see cref="MapNode.NumDice"/> to 1
        /// </summary>
        public void RandomlyAssignTerritories(bool overwriteDice = true)
        {
            // todo: optimize all this to avoid so much memory allocation and inefficient CPU usage
            
            Random rng = new Random();
            List<MapNode> shuffledNodes = Map.Nodes.Values.OrderBy(_ => rng.Next()).ToList();
            List<Player> shuffledPlayers = _players.ToList();
            
            for (var i = 0; i < shuffledNodes.Count; i++)
            {
                var playerIndex = i % _players.Count;
                if (playerIndex == 0)
                {
                    // reshuffle
                    shuffledPlayers = shuffledPlayers.OrderBy(_ => rng.Next()).ToList();
                }

                shuffledNodes[i].OwnerPlayerId = shuffledPlayers[playerIndex].PlayerID;
                if (overwriteDice)
                {
                    shuffledNodes[i].NumDice = 1;
                }
            }
        }

        public void RandomlyAllocateStartingReinforcements(int startingReinforcements)
        {
            if (Map.Nodes.Values.Any(n => n.OwnerPlayerId == -1))
            {
                Debug.Log("All map territories (nodes) must belong to a player before starting reinforcements are applied");
            }
            
            //todo
        }

        public void Reinforce(int playerId)
        {
            
        }

        public void Attack(int attackingSpaceId, int defendingSpaceId)
        {
            
        }

        public void EndTurn()
        {
            ActivePlayerIndex = (ActivePlayerIndex + 1) % Players.Count;
        }
    }
}
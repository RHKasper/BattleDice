using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleDataModel
{
    public class Battle
    {
        private readonly List<Player> _players;

        public int ActivePlayerIndex { get; private set; }
        public Map Map { get; private set; }
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
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
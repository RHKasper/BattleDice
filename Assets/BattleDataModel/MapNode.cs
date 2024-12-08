using System.Collections.Generic;
using System.Linq;

namespace BattleDataModel
{
    public class MapNode
    {
        internal readonly List<MapNode> AdjacentMapNodes = new();

        public int NodeId {get; internal set;}
        public int OwnerPlayerId { get; internal set; } = -1;
        public int NumDice { get; internal set; } = 0;

        public MapNode(int nodeId)
        {
            NodeId = nodeId;
        }

        public IReadOnlyList<MapNode> AdjacentNodes => AdjacentMapNodes;

        public bool CanAttack(int currentActivePlayerId)
        {
            bool isOwnedByActivePlayer = OwnerPlayerId == currentActivePlayerId;
            bool hasAnAdjacentEnemySpace = AdjacentNodes.Any(n => n.OwnerPlayerId != OwnerPlayerId);
            bool hasEnoughDice = NumDice > 1;

            return isOwnedByActivePlayer && hasAnAdjacentEnemySpace && hasEnoughDice;
        }

        public bool CanBeAttackedByAGivenNode(MapNode attackingNode)
        {
            bool isAdjacent = attackingNode.AdjacentNodes.Contains(this);
            bool attackingSpaceHasEnoughDice = attackingNode.NumDice > 1;
            bool isDifferentTeam = attackingNode.OwnerPlayerId != OwnerPlayerId;

            return isAdjacent && attackingSpaceHasEnoughDice && isDifferentTeam;
        }
    }
}
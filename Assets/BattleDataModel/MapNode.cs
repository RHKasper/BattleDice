using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleDataModel
{
    public class MapNode
    {
        internal readonly HashSet<MapNode> AdjacentMapNodes = new();

        public int NodeIndex { get; }
        public int OwnerPlayerIndex { get; internal set; } = -1;
        public int NumDice { get; internal set; } = 0;

        public MapNode(int nodeIndex)
        {
            NodeIndex = nodeIndex;
            //Debug.Log(NodeId);
        }
        
        public MapNode(int nodeIndex, int ownerPlayerIndex, int startingNumDice)
        {
            NodeIndex = nodeIndex;
            OwnerPlayerIndex = ownerPlayerIndex;
            NumDice = startingNumDice;
        }

        public IReadOnlyCollection<MapNode> AdjacentNodes => AdjacentMapNodes;

        public bool CanAttack(int currentActivePlayerId)
        {
            bool isOwnedByActivePlayer = OwnerPlayerIndex == currentActivePlayerId;
            bool hasAnAdjacentEnemySpace = AdjacentNodes.Any(n => n.OwnerPlayerIndex != OwnerPlayerIndex);
            bool hasEnoughDice = NumDice > 1;

            return isOwnedByActivePlayer && hasAnAdjacentEnemySpace && hasEnoughDice;
        }

        public bool CanBeAttackedByAGivenNode(MapNode attackingNode)
        {
            bool isAdjacent = attackingNode.AdjacentNodes.Contains(this);
            bool attackingSpaceHasEnoughDice = attackingNode.NumDice > 1;
            bool isDifferentTeam = attackingNode.OwnerPlayerIndex != OwnerPlayerIndex;

            return isAdjacent && attackingSpaceHasEnoughDice && isDifferentTeam;
        }

        public void LinkToNode(MapNode otherNode)
        {
            AdjacentMapNodes.Add(otherNode);
        }
    }
}
using System.Collections.Generic;

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
        
        public IReadOnlyList<MapNode> GetAdjacentMapNodes() => AdjacentMapNodes.AsReadOnly();
    }
}
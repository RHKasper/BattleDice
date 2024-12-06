using System.Collections.Generic;

namespace BattleDataModel
{
    public class MapNode
    {
        internal List<MapNode> AdjacentMapNodes = new();
        internal int OwnerPlayerId;
        internal int NumDice;
        internal int NodeId;
    }
}
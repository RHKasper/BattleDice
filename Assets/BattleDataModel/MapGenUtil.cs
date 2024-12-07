using System.Collections.Generic;

namespace BattleDataModel
{
    public static class MapGenUtil
    {
        public static Map GenerateSimpleMapAsLine(int numNodes)
        {
            int idCount = -1;
            var nodes = new List<MapNode>();

            for (int i = 0; i < numNodes; i++)
            {
                MapNode node = new MapNode(++idCount);
                nodes.Add(node);
            }
            
            LinkAsLine(nodes);

            return new Map(nodes[0]);
        }

        private static void LinkAsLine(List<MapNode> mapNodes)
        {
            for (int i = 0; i < mapNodes.Count - 1; i++)
            {
                mapNodes[i].AdjacentMapNodes.Add(mapNodes[i+1]);
                mapNodes[i+1].AdjacentMapNodes.Add(mapNodes[i]);
            }
        }
    }
}
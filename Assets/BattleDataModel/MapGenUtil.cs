using System.Collections.Generic;

namespace BattleDataModel
{
    public static class MapGenUtil
    {
        public static Map GenerateSimpleMap_LineOfLength4()
        {
            int idCount = -1;
            MapNode node0 = new MapNode(++idCount);
            MapNode node1 = new MapNode(++idCount);
            MapNode node2 = new MapNode(++idCount);
            MapNode node3 = new MapNode(++idCount);

            LinkAsLine(new List<MapNode> { node0, node1, node2, node3 });

            return new Map(node0);
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
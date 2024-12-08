using System.Collections.Generic;
using UnityEngine;

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

        public static Map GenerateCircleMap(int minimumNodeCount)
        {
            int nodeCount = 0;
            MapNode rootNode = new(nodeCount);
            Queue<MapNode> frontierNodes = new Queue<MapNode>();
            frontierNodes.Enqueue(rootNode);

            while (nodeCount + 1 < minimumNodeCount)
            {
                var currentNode = frontierNodes.Dequeue();
                var node1 = new MapNode(++nodeCount);
                var node2 = new MapNode(++nodeCount);
                //var node3 = new MapNode(++nodeCount);

                currentNode.AdjacentMapNodes.Add(node1);
                currentNode.AdjacentMapNodes.Add(node2);
                //currentNode.AdjacentMapNodes.Add(node3);
                
                node1.AdjacentMapNodes.Add(currentNode);
                node2.AdjacentMapNodes.Add(currentNode);
                //node3.AdjacentMapNodes.Add(currentNode);
                
                node1.AdjacentMapNodes.Add(node2);
                node2.AdjacentMapNodes.Add(node1);
                
                //node2.AdjacentMapNodes.Add(node3);
                //node3.AdjacentMapNodes.Add(node2);
                
                frontierNodes.Enqueue(node1);
                frontierNodes.Enqueue(node2);
                //frontierNodes.Enqueue(node3);
            }

            return new Map(rootNode);
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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleDataModel
{
    public static class MapValidator
    {
        public static void ValidateMapGraph(Map map)
        {
            CheckThatAllNodeIdsAreUnique(map);
            CheckThatNoNodeIsConnectedToItself(map);
            CheckThatAllConnectionsAreTwoWay(map);
            CheckThatAllNodesAreContiguous(map);
        }

        private static void CheckThatAllNodeIdsAreUnique(Map map)
        {
            HashSet<int> nodeIds = new HashSet<int>();

            foreach (MapNode node in map.Nodes.Values)
            {
                if (nodeIds.Contains(node.NodeId))
                {
                    throw new Exception("Node ID " + node.NodeId + " occurs more than once");
                }

                nodeIds.Add(node.NodeId);
            }
        }
        
        private static void CheckThatAllNodesAreContiguous(Map map)
        {
            int largestContiguousArea = map.GetLargestContiguousTerritory(null).Count;
            if (largestContiguousArea != map.Nodes.Count)
            {
                throw new Exception("Not all nodes in this map are connected. Largest contiguous territory is " + largestContiguousArea + " while there are " + map.Nodes.Count + " nodes total");
            }
        }

        private static void CheckThatNoNodeIsConnectedToItself(Map map)
        {
            foreach (MapNode node in map.Nodes.Values)
            {
                if (node.AdjacentMapNodes.Contains(node))
                {
                    throw new Exception("Node " + node.NodeId + " is connected to itself");
                }
            }
        }

        private static void CheckThatAllConnectionsAreTwoWay(Map map)
        {
            foreach (MapNode node in map.Nodes.Values)
            {
                foreach (MapNode adjacentMapNode in node.AdjacentMapNodes)
                {
                    if (!adjacentMapNode.AdjacentMapNodes.Contains(node))
                    {
                        throw new Exception("Node " + node.NodeId + " has a connection to node " + adjacentMapNode.NodeId + ", but node " + adjacentMapNode.NodeId + " doesn't have a connection to " + node.NodeId);
                    }
                }
            }
        }
    }
}
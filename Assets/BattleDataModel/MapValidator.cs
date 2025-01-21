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

            foreach (MapNode node in map.Territories.Values)
            {
                if (nodeIds.Contains(node.NodeIndex))
                {
                    throw new Exception("Node ID " + node.NodeIndex + " occurs more than once");
                }

                nodeIds.Add(node.NodeIndex);
            }
        }
        
        private static void CheckThatAllNodesAreContiguous(Map map)
        {
            int largestContiguousArea = map.GetLargestContiguousGroupOfTerritories(null).Count;
            if (largestContiguousArea != map.Territories.Count)
            {
                throw new Exception("Not all nodes in this map are connected. Largest contiguous territory is " + largestContiguousArea + " while there are " + map.Territories.Count + " nodes total");
            }
        }

        private static void CheckThatNoNodeIsConnectedToItself(Map map)
        {
            foreach (MapNode node in map.Territories.Values)
            {
                if (node.AdjacentMapNodes.Contains(node))
                {
                    throw new Exception("Node " + node.NodeIndex + " is connected to itself");
                }
            }
        }

        private static void CheckThatAllConnectionsAreTwoWay(Map map)
        {
            foreach (MapNode node in map.Territories.Values)
            {
                foreach (MapNode adjacentMapNode in node.AdjacentMapNodes)
                {
                    if (!adjacentMapNode.AdjacentMapNodes.Contains(node))
                    {
                        throw new Exception("Node " + node.NodeIndex + " has a connection to node " + adjacentMapNode.NodeIndex + ", but node " + adjacentMapNode.NodeIndex + " doesn't have a connection to " + node.NodeIndex);
                    }
                }
            }
        }
    }
}
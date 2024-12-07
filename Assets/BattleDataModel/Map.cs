using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleDataModel
{
    public class Map
    {
        private Dictionary<int, MapNode> _nodes;

        public IReadOnlyDictionary<int, MapNode> Nodes => _nodes;

        public Map(MapNode rootNode)
        {
            Stack<MapNode> nodesToProcess = new();
            nodesToProcess.Push(rootNode);
            
            _nodes = new Dictionary<int, MapNode>();

            while (nodesToProcess.Any())
            {
                MapNode currentNode = nodesToProcess.Pop();
                _nodes.Add(currentNode.NodeId, currentNode);

                foreach (MapNode adjacentMapNode in currentNode.AdjacentMapNodes)
                {
                    if (!_nodes.ContainsKey(adjacentMapNode.NodeId))
                    {
                        nodesToProcess.Push(adjacentMapNode);
                    }
                }
            }

            Debug.Log("Nodes processed: " + _nodes.Values.Count);
        }
        
        public HashSet<MapNode> GetLargestContiguousTerritory(int owningPlayerId)
        {
            HashSet<MapNode> largestYet = new();

            foreach (var node in _nodes.Values)
            {
                if (node.OwnerPlayerId == owningPlayerId && !largestYet.Contains(node))
                {
                    HashSet<MapNode> connectedNodes = new();
                    Stack<MapNode> frontierNodes = new Stack<MapNode>();
                    frontierNodes.Push(node);

                    while (frontierNodes.Count != 0)
                    {
                        var tempNode = frontierNodes.Pop();
                        connectedNodes.Add(tempNode);
                        
                        foreach (var adjacentNode in tempNode.AdjacentMapNodes)
                        {
                            if (adjacentNode.OwnerPlayerId == owningPlayerId && !connectedNodes.Contains(adjacentNode))
                            {
                                frontierNodes.Push(adjacentNode);
                            }
                        }
                    }
                    
                    Debug.Log("Contiguous group size: " + connectedNodes.Count);
                    if (connectedNodes.Count > largestYet.Count)
                    {
                        largestYet = connectedNodes;
                    }
                    
                }
            }

            return largestYet;
        }
    }
}

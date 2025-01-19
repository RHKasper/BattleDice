using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleDataModel
{
    public class Map
    {
        private readonly Dictionary<int, MapNode> _nodes;

        public IReadOnlyDictionary<int, MapNode> Nodes => _nodes;

        public Map(MapNode rootNode)
        {
            Stack<MapNode> nodesToProcess = new();
            nodesToProcess.Push(rootNode);
            
            _nodes = new Dictionary<int, MapNode>();

            while (nodesToProcess.Any())
            {
                MapNode currentNode = nodesToProcess.Pop();
                _nodes.Add(currentNode.NodeIndex, currentNode);

                foreach (MapNode adjacentMapNode in currentNode.AdjacentMapNodes)
                {
                    if (!nodesToProcess.Contains(adjacentMapNode) && !_nodes.ContainsKey(adjacentMapNode.NodeIndex))
                    {
                        nodesToProcess.Push(adjacentMapNode);
                    }
                }
            }
            
            MapValidator.ValidateMapGraph(this);
        }

        public HashSet<MapNode> GetTerritories(int owningPlayerId)
        {
            HashSet<MapNode> territories = new();

            foreach (var node in _nodes.Values)
            {
                if (node.OwnerPlayerIndex == owningPlayerId)
                {
                    territories.Add(node);
                }
            }

            return territories;
        }
        
        public HashSet<MapNode> GetLargestContiguousGroupOfTerritories(int? owningPlayerId)
        {
            HashSet<MapNode> largestYet = new();

            foreach (var node in _nodes.Values)
            {
                if ((!owningPlayerId.HasValue || node.OwnerPlayerIndex == owningPlayerId) && !largestYet.Contains(node))
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
                            if ((!owningPlayerId.HasValue || adjacentNode.OwnerPlayerIndex == owningPlayerId) && !connectedNodes.Contains(adjacentNode))
                            {
                                frontierNodes.Push(adjacentNode);
                            }
                        }
                    }
                    
                    //Debug.Log("Contiguous Area: " + connectedNodes.Count);
                    
                    if (connectedNodes.Count > largestYet.Count)
                    {
                        largestYet = connectedNodes;
                    }
                    
                }
            }

            return largestYet;
        }

        public List<MapNode> GetTerritoriesOwnedByPlayer(int playerIndex) => _nodes.Values.Where(n => n.OwnerPlayerIndex == playerIndex).ToList();
        public int GetNumTerritoriesOwnedByPlayer(int playerIndex) => _nodes.Values.Count(n => n.OwnerPlayerIndex == playerIndex);
    }
}

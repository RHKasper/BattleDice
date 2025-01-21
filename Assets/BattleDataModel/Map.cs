using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleDataModel
{
    public class Map
    {
        private readonly Dictionary<int, MapNode> _territories;

        public IReadOnlyDictionary<int, MapNode> Territories => _territories;

        public Map(MapNode rootNode)
        {
            Stack<MapNode> nodesToProcess = new();
            nodesToProcess.Push(rootNode);
            
            _territories = new Dictionary<int, MapNode>();

            while (nodesToProcess.Any())
            {
                MapNode currentNode = nodesToProcess.Pop();
                _territories.Add(currentNode.NodeIndex, currentNode);

                foreach (MapNode adjacentMapNode in currentNode.AdjacentMapNodes)
                {
                    if (!nodesToProcess.Contains(adjacentMapNode) && !_territories.ContainsKey(adjacentMapNode.NodeIndex))
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

            foreach (var node in _territories.Values)
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

            foreach (var node in _territories.Values)
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

        public List<MapNode> GetTerritoriesOwnedByPlayer(int playerIndex) => _territories.Values.Where(n => n.OwnerPlayerIndex == playerIndex).ToList();
        public int GetNumTerritoriesOwnedByPlayer(int playerIndex) => _territories.Values.Count(n => n.OwnerPlayerIndex == playerIndex);
    }
}

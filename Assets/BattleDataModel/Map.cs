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
    }
}

using System.Collections;
using System.Collections.Generic;
using BattleDataModel;
using UnityEngine;

namespace BattleTest
{
    public class BattleTester : MonoBehaviour
    {
        [SerializeField] private MapNodeVisualController mapNodeVisualPrefab;
        [SerializeField] private MapEdgeVisualController mapEdgeVisualPrefab;
        [SerializeField] private Transform edgesParent;
        [SerializeField] private Transform nodesParent;

        private readonly Dictionary<MapNode, MapNodeVisualController> _instantiatedMapNodeVisuals = new();
    
        private IEnumerator Start()
        {
            var map = MapGenUtil.GenerateSimpleMap_LineOfLength4();

            foreach (var mapNode in map.Nodes.Values)
            {
                var mapNodeVisual = Instantiate(mapNodeVisualPrefab, nodesParent);
                mapNodeVisual.Initialize(mapNode);
                _instantiatedMapNodeVisuals.Add(mapNode, mapNodeVisual);
            }
            
            yield return new WaitForEndOfFrame();
            
            foreach (var mapNode in map.Nodes.Values)
            {
                foreach (MapNode adjacentNode in mapNode.GetAdjacentMapNodes())
                {
                    Instantiate(mapEdgeVisualPrefab, edgesParent).Initialize(_instantiatedMapNodeVisuals[mapNode], _instantiatedMapNodeVisuals[adjacentNode]);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using BattleDataModel;
using RKUnityToolkit.UnityExtensions;
using UnityEngine;

namespace Maps
{
    public class GameplayMap : MonoBehaviour
    {
        [SerializeField] private Transform territoriesParent;

        public RectTransform RectTransform => GetComponent<RectTransform>();

        private Dictionary<MapNode, GameObject> _territoryGameObjects = new();

        public Map GenerateMapData()
        {
            GameplayMapNodeDefinition[] nodeDefinitions = GetNodeDefinitionsInOrder();
            Dictionary<GameplayMapNodeDefinition, int> indices = new();
            MapNode[] nodes = new MapNode[nodeDefinitions.Length];

            for (int i = 0; i < nodeDefinitions.Length; i++)
            {
                nodes[i] = CreateMapNode(nodeDefinitions[i], i);
                indices[nodeDefinitions[i]] = i;
                _territoryGameObjects.Add(nodes[i], nodeDefinitions[i].gameObject);
            }

            for (int i = 0; i < nodeDefinitions.Length; i++)
            {
                foreach (var adjacentNodeDefinition in nodeDefinitions[i].adjacentNodes)
                {
                    nodes[i].LinkToNode(nodes[indices[adjacentNodeDefinition]]);
                }
            }

            return new Map(nodes[0]);
        }

        public GameplayMapNodeDefinition[] GetNodeDefinitionsInOrder()
        {
            return territoriesParent.GetComponentsInDirectChildren<GameplayMapNodeDefinition>();
        }

        public GameObject GetTerritoryGameObject(MapNode territory)
        {
            return _territoryGameObjects[territory];
        }

        protected virtual MapNode CreateMapNode(GameplayMapNodeDefinition definition, int index)
        {
            return new MapNode(index);
        }
    }
}
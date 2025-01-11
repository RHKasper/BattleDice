using System;
using System.Collections.Generic;
using BattleDataModel;
using RKUnityToolkit.UnityExtensions;
using UnityEngine;

namespace Maps
{
    public class GameplayMap : MonoBehaviour
    {
        public RectTransform RectTransform => GetComponent<RectTransform>();

        public Map GenerateMapData()
        {
            GameplayMapNodeDefinition[] nodeDefinitions = GetNodeDefinitionsInOrder();
            Dictionary<GameplayMapNodeDefinition, int> indices = new();
            MapNode[] nodes = new MapNode[nodeDefinitions.Length];

            for (int i = 0; i < nodeDefinitions.Length; i++)
            {
                nodes[i] = new MapNode(i);
                indices[nodeDefinitions[i]] = i;
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
            return gameObject.GetComponentsInDirectChildren<GameplayMapNodeDefinition>();
        }
    }
}
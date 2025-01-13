using RKUnityToolkit.UnityExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maps.Helpers
{
    public class HexMapMaker : MonoBehaviour
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private GameplayMapNodeDefinition prefab;
        [SerializeField] private float tileWidthAndHeight;
        [SerializeField] private int columns;
        [SerializeField] private int height;

        [ContextMenu("Create Hex Board")]
        public void CreateHexBoard()
        {
            #if UNITY_EDITOR
            GameObject boardParent = new GameObject("Generated Hex Board (" + columns + " by " + height + ")");
            boardParent.transform.SetParent(parent);
            RectTransform parentRectTransform = boardParent.AddComponent<RectTransform>();
            parentRectTransform.anchorMin = Vector2.zero;
            parentRectTransform.anchorMax = Vector2.one;
            parentRectTransform.offsetMin = Vector2.zero;
            parentRectTransform.offsetMax = Vector2.zero;
            
            for (int i = 0; i < columns; i++)
            {
                int colHeight = i % 2 == 0 ? height : height - 1;
                float xPosition = tileWidthAndHeight * (.5f + .75f * i);

                for (int j = 0; j < colHeight; j++)
                {
                    float yPosition = tileWidthAndHeight * ((i % 2 == 0 ? .5f : 1) + j);
                    
                    GameplayMapNodeDefinition tile = (GameplayMapNodeDefinition)PrefabUtility.InstantiatePrefab(prefab, parentRectTransform);
                    RectTransform tileRectTransform = tile.GetComponent<RectTransform>();
                    tileRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    tileRectTransform.anchorMin = Vector2.zero;
                    tileRectTransform.anchorMax = Vector2.zero;
                    tileRectTransform.anchoredPosition = new Vector2(xPosition, yPosition); 
                }
            }

            ConnectAdjacents(parentRectTransform);
            #endif
        }

        // this is super inefficient, and I don't think it's worth optimizing
        private void ConnectAdjacents(Transform parentTransform)
        {
            GameplayMapNodeDefinition[] nodes = parentTransform.GetComponentsInDirectChildren<GameplayMapNodeDefinition>();

            float maxSqrDistance = tileWidthAndHeight * tileWidthAndHeight;
            
            foreach (GameplayMapNodeDefinition node in nodes)
            {
                foreach (GameplayMapNodeDefinition maybeAdjNode in nodes)
                {
                    if (node != maybeAdjNode)
                    {
                        float sqrDistance = (node.GetComponent<RectTransform>().anchoredPosition - maybeAdjNode.GetComponent<RectTransform>().anchoredPosition).sqrMagnitude;
                        if (sqrDistance <= maxSqrDistance)
                        {
                            // connect
                            node.adjacentNodes.Add(maybeAdjNode);
                        }
                    }
                }
            }
        }
    }
}

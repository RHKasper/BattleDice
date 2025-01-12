using BattleRunner;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maps.Helpers
{
    [ExecuteAlways]
    public class MapCreationHelper : MonoBehaviour
    {
        [SerializeField] private GameplayMap activeMap;
        [SerializeField] private Transform edgesParent;
        [SerializeField] private EdgeVisualControllerBase edgeVisualPrefab;
        
        private void Update()
        {
            if (!Application.isPlaying)
            {
                GameplayMapNodeDefinition[] nodes = activeMap.GetNodeDefinitionsInOrder();

                EnsureEachNodeHasAVisualController(nodes);
                EnsureEachConnectionGoesBothWays(nodes);
                EnsureEachNodesVisualControllerHasAppropriateEdgeVisualControllers(nodes);
            }
        }

        private void EnsureEachNodeHasAVisualController(GameplayMapNodeDefinition[] nodes)
        {
            foreach (GameplayMapNodeDefinition node in nodes)
            {
                TerritoryVisualControllerBase visualController = node.GetComponent<TerritoryVisualControllerBase>();
                if (visualController == null)
                {
                    Debug.LogError("Could not find Territory VisualController. Adding a BasicTerritoryVisualController to " + node.gameObject.name);
                    node.gameObject.AddComponent<BasicTerritoryVisualController>();
                }
            }
        }
        
        private void EnsureEachConnectionGoesBothWays(GameplayMapNodeDefinition[] nodes)
        {
            foreach (GameplayMapNodeDefinition node in nodes)
            {
                foreach (GameplayMapNodeDefinition adjacentNode in node.adjacentNodes)
                {
                    if (!adjacentNode.adjacentNodes.Contains(node))
                    {
                        adjacentNode.adjacentNodes.Add(node);
#if UNITY_EDITOR
                        EditorUtility.SetDirty(adjacentNode);
#endif
                    }
                }
            }
        }

        private void EnsureEachNodesVisualControllerHasAppropriateEdgeVisualControllers(GameplayMapNodeDefinition[] nodes)
        {
#if UNITY_EDITOR
            foreach (GameplayMapNodeDefinition node in nodes)
            {
                var visualController = node.GetComponent<TerritoryVisualControllerBase>();
                foreach (GameplayMapNodeDefinition adjacentNode in node.adjacentNodes)
                {
                    var adjacentVisualController = adjacentNode.GetComponent<TerritoryVisualControllerBase>();
                    visualController.edges.TryGetValue(adjacentVisualController, out var myEdge);
                    adjacentVisualController.edges.TryGetValue(visualController, out var theirEdge);

                    if (myEdge == null && theirEdge == null)
                    {
                        visualController.edges[adjacentVisualController] = (EdgeVisualControllerBase) PrefabUtility.InstantiatePrefab(edgeVisualPrefab, edgesParent);
                        adjacentVisualController.edges[visualController] = visualController.edges[adjacentVisualController];
                    }
                    else if (myEdge == null)
                    {
                        visualController.edges[adjacentVisualController] = theirEdge;
                    }
                    else if (theirEdge == null)
                    {
                        adjacentVisualController.edges[visualController] = myEdge;
                    }
                    else if(myEdge != theirEdge)
                    {
                        Debug.LogError("Mismatched edge references on " + node.gameObject.name + " and " + adjacentNode.gameObject.name);
                    }

                    // ensure edge controller points to nodes
                    var edge = visualController.edges[adjacentVisualController];
                    if (!edge.Connects(visualController, adjacentVisualController))
                    {
                        edge.OverrideEnds(visualController, adjacentVisualController);
                    }
                }
            }
#endif
        }
    }
}
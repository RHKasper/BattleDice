using BattleRunner;
using UnityEngine;

namespace Maps.Helpers
{
    [ExecuteAlways]
    public class MapCreationHelper : MonoBehaviour
    {
        [SerializeField] GameplayMap activeMap;
        
        private void Update()
        {
            GameplayMapNodeDefinition[] nodes = activeMap.GetNodeDefinitionsInOrder();

            EnsureEachNodeHasAVisualController(nodes);
            EnsureEachNodesVisualControllerHasAppropriateEdges(nodes);
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

        private void EnsureEachNodesVisualControllerHasAppropriateEdges(GameplayMapNodeDefinition[] nodes)
        {
            foreach (GameplayMapNodeDefinition node in nodes)
            {
                var visualController = node.GetComponent<TerritoryVisualControllerBase>();
                foreach (GameplayMapNodeDefinition adjacentNode in node.adjacentNodes)
                {
                    var adjacentVisualController = adjacentNode.GetComponent<TerritoryVisualControllerBase>();
                    if (!visualController.edges.ContainsKey(adjacentVisualController))
                    {
                        visualController.edges.Add(adjacentVisualController, null);
                    }

                    if (!adjacentVisualController.edges.ContainsKey(visualController))
                    {
                        adjacentVisualController.edges.Add(visualController, null);
                    }
                    
                    // var myEdge = visualController.edges[adjacentVisualController];
                    // var theirEdge = adjacentVisualController.edges[adjacentVisualController];
                    //
                    // if (myEdge == null)
                    // {
                    //     if (theirEdge == null)
                    //     {
                    //         // both null; create a new one
                    //     }
                    //     else
                    //     {
                    //         visualController.edges[adjacentVisualController] = theirEdge;
                    //     }
                    // }
                    // else if (theirEdge == null)
                    // {
                    //     adjacentVisualController.edges[adjacentVisualController] = myEdge;
                    // }
                    
                    // ensure edge controller points to nodes
                }
            }
        }
    }
}
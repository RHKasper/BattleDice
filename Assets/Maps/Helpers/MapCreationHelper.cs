using BattleRunner;
using UnityEngine;

namespace Maps.EditorTools
{
    [ExecuteAlways]
    public class MapCreationHelper : MonoBehaviour
    {
        [SerializeField] GameplayMap activeMap;
        
        private void Update()
        {
            foreach (GameplayMapNodeDefinition node in activeMap.GetNodeDefinitionsInOrder())
            {
                // ensure that node has a visual controller
                TerritoryVisualControllerBase visualController = node.GetComponent<TerritoryVisualControllerBase>();
                if (visualController == null)
                {
                    Debug.LogError("Could not find Territory VisualController. Adding a BasicTerritoryVisualController to " + node.gameObject.name);
                    visualController = node.gameObject.AddComponent<BasicTerritoryVisualController>();
                }
                
                // ensure that visual controller has an edge visual controller for each adjacent node
                // ensure that edge controller points to the other node
                // ensure the other node is correctly linked to this node/edge
            }
        }
    }
}
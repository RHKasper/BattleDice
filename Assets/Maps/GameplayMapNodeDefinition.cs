using System.Collections.Generic;
using BattleDataModel;
using UnityEngine;

namespace Maps
{
    public class GameplayMapNodeDefinition : MonoBehaviour
    {
        public List<GameplayMapNodeDefinition> adjacentNodes;
        
        public MapNode RuntimeData { get; set; }

        private void OnDrawGizmos()
        {
            foreach (GameplayMapNodeDefinition adjacentNode in adjacentNodes)
            {
                Gizmos.DrawLine(transform.position, adjacentNode.transform.position);
                if (!adjacentNode.adjacentNodes.Contains(this))
                {
                    adjacentNode.adjacentNodes.Add(this);
                        
                }
            }
        }
    }
}
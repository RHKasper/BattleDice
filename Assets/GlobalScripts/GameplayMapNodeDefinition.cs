using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlobalScripts
{
    public class GameplayMapNodeDefinition : MonoBehaviour
    {
        public List<GameplayMapNodeDefinition> adjacentNodes;

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
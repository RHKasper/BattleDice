using System.Collections.Generic;
using BattleDataModel;
using UnityEngine;

namespace Maps
{
    public class GameplayMapNodeDefinition : MonoBehaviour
    {
        public List<GameplayMapNodeDefinition> adjacentNodes;
        
        public MapNode RuntimeData { get; set; }
    }
}
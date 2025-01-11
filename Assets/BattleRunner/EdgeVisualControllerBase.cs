using UnityEngine;

namespace BattleRunner
{
    public abstract class EdgeVisualControllerBase : MonoBehaviour
    {
        public RectTransform RectTransform => GetComponent<RectTransform>();
        
        public abstract void Initialize(TerritoryVisualControllerBase end1, TerritoryVisualControllerBase end2);
        // todo: add abstract interaction visual methods
    }
}

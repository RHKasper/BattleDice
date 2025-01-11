using UnityEngine;

namespace BattleRunner
{
    public abstract class EdgeVisualControllerBase : MonoBehaviour
    {
        [SerializeField] protected TerritoryVisualControllerBase end1;
        [SerializeField] protected TerritoryVisualControllerBase end2;

        public RectTransform RectTransform => GetComponent<RectTransform>();
        
        public abstract void Initialize();
        // todo: add abstract interaction visual methods
    }
}

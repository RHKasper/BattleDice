using UnityEngine;

namespace BattleRunner
{
    public abstract class EdgeVisualControllerBase : MonoBehaviour
    {
        [SerializeField] protected TerritoryVisualControllerBase end1;
        [SerializeField] protected TerritoryVisualControllerBase end2;

        public RectTransform RectTransform => GetComponent<RectTransform>();

        public bool Connects(TerritoryVisualControllerBase node1, TerritoryVisualControllerBase node2)
        {
            return (end1 == node1 && end2 == node2) || (end1 == node2 && end2 == node1);
        }

        public void OverrideEnds(TerritoryVisualControllerBase node1, TerritoryVisualControllerBase node2)
        {
            end1 = node1;
            end2 = node2;
        }
        
        public abstract void Initialize();
        // todo: add abstract interaction visual methods
    }
}

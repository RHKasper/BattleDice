using UnityEngine;

namespace BattleRunner
{
    public class BasicEdgeVisualController : EdgeVisualControllerBase
    {
        public override void Initialize(TerritoryVisualControllerBase end1, TerritoryVisualControllerBase end2)
        {
            RectTransform.position = (end1.transform.position + end2.transform.position) / 2;
            Vector3 dif = end1.transform.position - end2.transform.position;
            RectTransform.sizeDelta = new Vector3(dif.magnitude, 5);
            RectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }
}

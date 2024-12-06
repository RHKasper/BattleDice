using UnityEngine;

namespace BattleTest
{
    public class MapEdgeVisualController : MonoBehaviour
    {
        public void Initialize(MapNodeVisualController node1, MapNodeVisualController node2)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.position = (node1.transform.position + node2.transform.position) / 2;
            
            Vector3 dif = node1.transform.position - node2.transform.position;
            rectTransform.sizeDelta = new Vector3(dif.magnitude, 5);
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }
}

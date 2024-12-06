using System;
using UnityEngine;

namespace BattleTest
{
    public class MapEdgeVisualController : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private MapNodeVisualController _node1;
        private MapNodeVisualController _node2;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(MapNodeVisualController node1, MapNodeVisualController node2)
        {
            _node1 = node1;
            _node2 = node2;
        }

        private void Update()
        {
            _rectTransform.position = (_node1.transform.position + _node2.transform.position) / 2;
            Vector3 dif = _node1.transform.position - _node2.transform.position;
            _rectTransform.sizeDelta = new Vector3(dif.magnitude, 5);
            _rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }
}

using UnityEngine;

namespace BattleTest.MapVisuals
{
    public class MapEdgeVisualController : MonoBehaviour
    {
        [SerializeField] private GameObject highlightVisuals;
        
        private RectTransform _rectTransform;
        private MapNodeVisualController _node1;
        private MapNodeVisualController _node2;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            highlightVisuals.SetActive(false);
        }

        public void Initialize(MapNodeVisualController node1, MapNodeVisualController node2)
        {
            _node1 = node1;
            _node2 = node2;
            _node1.RegisterEdgeVisual(_node2, this);
            _node2.RegisterEdgeVisual(_node1, this);
        }

        private void Update()
        {
            _rectTransform.position = (_node1.transform.position + _node2.transform.position) / 2;
            Vector3 dif = _node1.transform.position - _node2.transform.position;
            _rectTransform.sizeDelta = new Vector3(dif.magnitude, 5);
            _rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }

        public void ShowPotentialAttackVisuals(MapNodeVisualController attackingNode)
        {
            MapNodeVisualController defendingNode = (_node1 == attackingNode) ? _node2 : _node1;
            defendingNode.ShowOrHideHighlightsAsPotentialAttackTarget(true);
            highlightVisuals.SetActive(true);
        }

        public void HidePotentialAttackVisuals(MapNodeVisualController attackingNode)
        {
            MapNodeVisualController defendingNode = (_node1 == attackingNode) ? _node2 : _node1;
            defendingNode.ShowOrHideHighlightsAsPotentialAttackTarget(false);
            highlightVisuals.SetActive(false);
        }
    }
}

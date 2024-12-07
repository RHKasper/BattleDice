using BattleDataModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleTest.Scripts
{
    public class MapNodeVisualController : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image diceFill;
        [SerializeField] private TextMeshProUGUI text;

        private MapNode _mapNode;

        private void Update()
        {
            text.text = $"ID: {_mapNode.NodeId}\nOwnerID: {_mapNode.OwnerPlayerId}\nDice: {_mapNode.NumDice}";
            diceFill.fillAmount = _mapNode.NumDice / 8f;

            if (_mapNode.OwnerPlayerId != -1)
            {
                background.color = PlayerColors.Colors[_mapNode.OwnerPlayerId];
            }
        }

        public void Initialize(MapNode mapNode)
        {
            _mapNode = mapNode;
        }
    }
}

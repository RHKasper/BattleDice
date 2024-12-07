using System;
using BattleDataModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleTest
{
    public class MapNodeVisualController : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;

        private MapNode _mapNode;

        private void Update()
        {
            text.text = $"ID: {_mapNode.NodeId}\nOwnerID: {_mapNode.OwnerPlayerId}\nDice: {_mapNode.NumDice}";

            if (_mapNode.OwnerPlayerId != -1)
            {
                image.color = PlayerColors.Colors[_mapNode.OwnerPlayerId];
            }
        }

        public void Initialize(MapNode mapNode)
        {
            _mapNode = mapNode;
        }
    }
}

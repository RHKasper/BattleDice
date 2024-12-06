using System;
using BattleDataModel;
using TMPro;
using UnityEngine;

namespace BattleTest
{
    public class MapNodeVisualController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private MapNode _mapNode;

        private void Update()
        {
            text.text = $"ID: {_mapNode.NodeId}\nOwnerID: {_mapNode.OwnerPlayerId}\nDice: {_mapNode.NumDice}";
        }

        public void Initialize(MapNode mapNode)
        {
            _mapNode = mapNode;
        }
    }
}

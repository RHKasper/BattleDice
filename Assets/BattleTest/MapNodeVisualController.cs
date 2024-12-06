using BattleDataModel;
using TMPro;
using UnityEngine;

namespace BattleTest
{
    public class MapNodeVisualController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void Initialize(MapNode mapNode)
        {
            text.text = $"ID: {mapNode.NodeId}\nOwnerID: {mapNode.OwnerPlayerId}\nDice: {mapNode.NumDice}";
        }
    }
}

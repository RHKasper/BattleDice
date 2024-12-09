using BattleDataModel;
using BattleTest.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleTest.PlayersStatusPanel
{
    public class PlayerStatusBoxController : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI text;

        public void SetData(int playerId, Map map)
        {
            int territories = map.GetTerritories(playerId).Count;
            int reinforcementCount = map.GetLargestContiguousTerritory(playerId).Count;
            
            backgroundImage.color = PlayerColors.Colors[playerId];
            text.text = "Player ID: " + playerId + "\nTerritories: " + territories + "\n\nReinforcements: " + reinforcementCount;
        }
    }
}

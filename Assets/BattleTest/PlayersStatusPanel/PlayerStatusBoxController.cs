using System;
using System.Linq;
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
        [SerializeField] private Image borderHighlight;
        [SerializeField] private TextMeshProUGUI text;
        
        public void SetData(int playerId, Map map)
        {
            var territories = map.GetTerritories(playerId);
            int reinforcementCount = map.GetLargestContiguousTerritory(playerId).Count;
            int totalDice = territories.Sum(t => t.NumDice);
            
            backgroundImage.color = PlayerColors.Colors[playerId];
            text.text = "Player ID: " + playerId + "\nTerritories: " + territories.Count + "\nTotal Dice: " + totalDice + "\nReinforcements: " + reinforcementCount;
        }

        public void SetHighlightActive(bool active) => borderHighlight.gameObject.SetActive(active);
    }
}

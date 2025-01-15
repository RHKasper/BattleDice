using System.Linq;
using BattleDataModel;
using GlobalScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace BattleRunner.UI
{
    public class PlayerInfoBoxController : MonoBehaviour
    {
        [SerializeField] private ProceduralImage border;
        [SerializeField] private Image dieImage;
        [SerializeField] private GameObject winnerVisuals;
        [SerializeField] private TextMeshProUGUI text;
        
        public void SetData(int playerIndex, Map map)
        {
            Debug.Log("Setting player info box for player ID " + playerIndex);
            var territories = map.GetTerritories(playerIndex);
            int reinforcementCount = map.GetLargestContiguousGroupOfTerritories(playerIndex).Count;
            int totalDice = territories.Sum(t => t.NumDice);

            dieImage.sprite = Resources.Load<Sprite>(Constants.GetThreeQuartersDieSpritesPathFromResources(playerIndex));
            border.color = Constants.Colors[playerIndex];
            text.text = territories.Count + "|" + reinforcementCount;
        }

        public void SetHighlightActive(bool active) => border.gameObject.SetActive(active);
        public void SetEliminatedVisualsActive(bool active) => gameObject.SetActive(active);
        public void SetWinnerVisualsActive(bool active) => winnerVisuals.SetActive(active);
    }
}

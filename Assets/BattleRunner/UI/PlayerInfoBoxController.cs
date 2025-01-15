using System.Collections.Generic;
using System.Linq;
using BattleDataModel;
using GlobalScripts;
using Maps;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace BattleRunner.UI
{
    public class PlayerInfoBoxController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ProceduralImage border;
        [SerializeField] private ProceduralImage activePlayerBorder;
        [SerializeField] private Image dieImage;
        [SerializeField] private GameObject winnerVisuals;
        [SerializeField] private TextMeshProUGUI text;

        private int _playerIndex;
        private GameplayMap _gameplayMap;
        private Battle _battle;

        public void Initialize(int playerIndex, GameplayMap gameplayMap, Battle battle)
        {
            _playerIndex = playerIndex;
            _gameplayMap = gameplayMap;
            _battle = battle;
            RefreshData();
        }

        public void RefreshData()
        {
            var territories = _battle.Map.GetTerritories(_playerIndex);
            int reinforcementCount = _battle.Map.GetLargestContiguousGroupOfTerritories(_playerIndex).Count;

            dieImage.sprite = Resources.Load<Sprite>(Constants.GetThreeQuartersDieSpritesPathFromResources(_playerIndex));
            border.color = Constants.Colors[_playerIndex];
            activePlayerBorder.color = Constants.Colors[_playerIndex];
            text.text = territories.Count + " | " + reinforcementCount;
            
            activePlayerBorder.gameObject.SetActive(_battle.ActivePlayer.PlayerIndex == _playerIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HashSet<MapNode> territories = _battle.Map.GetLargestContiguousGroupOfTerritories(_playerIndex);
            foreach (MapNode territory in territories)
            {
                var visualController = _gameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                visualController.HighlightedToShowLargestContiguousGroupOfTerritories = true;
                visualController.UpdateState();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (MapNode territory in _battle.Map.Nodes.Values)
            {
                var visualController = _gameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                visualController.HighlightedToShowLargestContiguousGroupOfTerritories = false;
                visualController.UpdateState();
            }
        }
    }
}

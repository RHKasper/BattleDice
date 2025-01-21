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
        [SerializeField] private Image background;
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
            bool isActivePlayer = _battle.ActivePlayer.PlayerIndex == _playerIndex;
            
            dieImage.sprite = Resources.Load<Sprite>(Constants.GetThreeQuartersDieSpritesPathFromResources(_playerIndex));
            border.color = isActivePlayer ? Color.white : Constants.Colors[_playerIndex];
            background.color = isActivePlayer ? Constants.Colors[_playerIndex] : Color.white;
            text.text = territories.Sum(t => t.NumDice) + " | " + territories.Count + " | " + reinforcementCount;

            gameObject.SetActive(!_battle.GetPlayer(_playerIndex).Eliminated);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                HashSet<MapNode> territories = _battle.Map.GetLargestContiguousGroupOfTerritories(_playerIndex);
                foreach (MapNode territory in territories)
                {
                    var visualController = _gameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                    visualController.HighlightedToShowLargestContiguousGroupOfTerritories = true;
                    visualController.UpdateState();
                }    
            }, "Showing largest Contiguous Group Of Territories");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                foreach (MapNode territory in _battle.Map.Territories.Values)
                {
                    var visualController = _gameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                    visualController.HighlightedToShowLargestContiguousGroupOfTerritories = false;
                    visualController.UpdateState();
                }
            }, "Hiding largest Contiguous Group Of Territories");
        }
    }
}

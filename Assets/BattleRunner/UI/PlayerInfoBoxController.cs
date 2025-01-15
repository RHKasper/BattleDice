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
        [SerializeField] private Image dieImage;
        [SerializeField] private GameObject winnerVisuals;
        [SerializeField] private TextMeshProUGUI text;

        private int _playerIndex;
        private GameplayMap _gameplayMap;
        private Map _map;

        public void Initialize(int playerIndex, GameplayMap gameplayMap, Map map)
        {
            _playerIndex = playerIndex;
            _gameplayMap = gameplayMap;
            _map = map;
            RefreshData();
        }

        public void RefreshData()
        {
            var territories = _map.GetTerritories(_playerIndex);
            int reinforcementCount = _map.GetLargestContiguousGroupOfTerritories(_playerIndex).Count;

            dieImage.sprite = Resources.Load<Sprite>(Constants.GetThreeQuartersDieSpritesPathFromResources(_playerIndex));
            border.color = Constants.Colors[_playerIndex];
            text.text = territories.Count + "|" + reinforcementCount;
        }
        
        public void SetData(int playerIndex, Map map)
        {
            // todo: delete
        }

        public void SetHighlightActive(bool active) => border.gameObject.SetActive(active);
        public void SetEliminatedVisualsActive(bool active) => gameObject.SetActive(active);
        public void SetWinnerVisualsActive(bool active) => winnerVisuals.SetActive(active);
        public void OnPointerEnter(PointerEventData eventData)
        {
            HashSet<MapNode> territories = _map.GetLargestContiguousGroupOfTerritories(_playerIndex);
            foreach (MapNode territory in territories)
            {
                var visualController = _gameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                visualController.HighlightedToShowLargestContiguousGroupOfTerritories = true;
                visualController.UpdateState();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (MapNode territory in _map.Nodes.Values)
            {
                var visualController = _gameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                visualController.HighlightedToShowLargestContiguousGroupOfTerritories = false;
                visualController.UpdateState();
            }
        }
    }
}

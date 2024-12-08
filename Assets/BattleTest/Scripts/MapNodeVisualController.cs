using System.Linq;
using BattleDataModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleTest.Scripts
{
    public class MapNodeVisualController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D interactableCursor;
        
        [SerializeField] private Image background;
        [SerializeField] private Image diceFill;
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private TextMeshProUGUI text;

        private MapNode _mapNode;
        private BattleTester _battleTester;
        
        private bool ThisTerritoryIsSelected => _battleTester.SelectedNode == this;
        private bool ATerritoryIsSelected => _battleTester.SelectedNode != null;
        private bool CanAttack => _mapNode.CanAttack(_battleTester.Battle.ActivePlayer.PlayerID);
        private bool CanBeAttacked => ATerritoryIsSelected && _mapNode.CanBeAttackedByAGivenNode(_battleTester.SelectedNode._mapNode);
        private bool IsInteractable => (!ATerritoryIsSelected && CanAttack) || (ATerritoryIsSelected && CanBeAttacked) || ThisTerritoryIsSelected;

        private void Update()
        {
            text.text = $"ID: {_mapNode.NodeId}\nOwnerID: {_mapNode.OwnerPlayerId}\nDice: {_mapNode.NumDice}";
            diceFill.fillAmount = _mapNode.NumDice / 8f;

            if (_mapNode.OwnerPlayerId != -1)
            {
                background.color = PlayerColors.Colors[_mapNode.OwnerPlayerId];
            }
        }

        public void Initialize(MapNode mapNode, BattleTester battleTester)
        {
            _mapNode = mapNode;
            _battleTester = battleTester;
            selectionHighlight.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Cursor.SetCursor(IsInteractable ? interactableCursor : defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ATerritoryIsSelected && CanBeAttacked)
            {
                Debug.Log("Todo: execute attack");
            }
            else if (!ATerritoryIsSelected && CanAttack)
            {
                _battleTester.SelectedNode = this;
            }
            else if (ThisTerritoryIsSelected)
            {
                _battleTester.SelectedNode = null;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        public void OnDeselected()
        {
            selectionHighlight.gameObject.SetActive(false);
        }

        public void OnSelected()
        {
            //highlight attackable edges / nodes
            selectionHighlight.gameObject.SetActive(true);
        }
    }
}

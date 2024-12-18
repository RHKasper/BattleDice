using System.Collections.Generic;
using BattleDataModel;
using BattleTest.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleTest.MapVisuals
{
    public class MapNodeVisualController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D interactableCursor;

        [SerializeField] public RectTransform rectTransform;
        [SerializeField] private Image background;
        [SerializeField] private Image diceFill;
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private TextMeshProUGUI text;

        private MapNode _mapNode;
        private BattleTester _battleTester;
        private Dictionary<MapNode, MapEdgeVisualController> _edgeVisuals = new(); 
        
        private bool ThisTerritoryIsSelected => _battleTester.SelectedNode == this;
        private bool ATerritoryIsSelected => _battleTester.SelectedNode != null;
        private bool CanAttack => _mapNode.CanAttack(_battleTester.Battle.ActivePlayer.PlayerIndex);
        private bool CanBeAttacked => ATerritoryIsSelected && _mapNode.CanBeAttackedByAGivenNode(_battleTester.SelectedNode._mapNode);
        private bool IsInteractable => (!ATerritoryIsSelected && CanAttack) || (ATerritoryIsSelected && CanBeAttacked) || ThisTerritoryIsSelected;

        private void Update()
        {
            text.text = $"ID: {_mapNode.NodeIndex}\nOwnerID: {_mapNode.OwnerPlayerIndex}\nDice: {_mapNode.NumDice}";
            diceFill.fillAmount = _mapNode.NumDice / 8f;

            if (_mapNode.OwnerPlayerIndex != -1)
            {
                background.color = PlayerColors.Colors[_mapNode.OwnerPlayerIndex];
            }
        }

        public void Initialize(MapNode mapNode, BattleTester battleTester)
        {
            _mapNode = mapNode;
            _battleTester = battleTester;
            selectionHighlight.gameObject.SetActive(false);
        }

        public bool HasEdgeVisual(MapNode adjacentNode)
        {
            return _edgeVisuals.ContainsKey(adjacentNode);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Cursor.SetCursor(IsInteractable ? interactableCursor : defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ATerritoryIsSelected && CanBeAttacked)
            {
                _battleTester.Battle.Attack(_battleTester.SelectedNode._mapNode, _mapNode);
                _battleTester.SelectedNode = null;
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
            SetPotentialAttackTargetHighlightActive(false);
        }

        public void OnSelected()
        {
            selectionHighlight.gameObject.SetActive(true);
            SetPotentialAttackTargetHighlightActive(true);
        }

        public void RegisterEdgeVisual(MapNodeVisualController node2, MapEdgeVisualController edgeVisual)
        {
            _edgeVisuals.Add(node2._mapNode, edgeVisual);
        }

        private void SetPotentialAttackTargetHighlightActive(bool active)
        {
            foreach (MapNode adjSpace in _edgeVisuals.Keys)
            {
                _edgeVisuals[adjSpace].SetHighlightVisualsActive(active && adjSpace.CanBeAttackedByAGivenNode(_mapNode));
            }
        }
    }
}

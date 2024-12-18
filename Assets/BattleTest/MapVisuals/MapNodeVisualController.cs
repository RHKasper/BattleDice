using System;
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
        [SerializeField] private Texture2D interactableCursor;
        [SerializeField] private Vector2 interactableCursorHotspotOffset;

        [SerializeField] public RectTransform rectTransform;
        [SerializeField] private Image background;
        [SerializeField] private Image diceFill;
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private Image targetHighlight;
        [SerializeField] private TextMeshProUGUI text;

        private MapNode _mapNode;
        private BattleTester _battleTester;
        private Dictionary<MapNode, MapEdgeVisualController> _edgeVisuals = new(); 
        
        private bool ThisTerritoryIsSelected => _battleTester.SelectedNode == this;
        private bool ATerritoryIsSelected => _battleTester.SelectedNode != null;
        private bool CanAttack => _mapNode.CanAttack(_battleTester.Battle.ActivePlayer.PlayerIndex);
        private bool CanBeAttacked => ATerritoryIsSelected && _mapNode.CanBeAttackedByAGivenNode(_battleTester.SelectedNode._mapNode);
        private bool IsInteractable => (!ATerritoryIsSelected && CanAttack) || (ATerritoryIsSelected && CanBeAttacked) || ThisTerritoryIsSelected;

        private void Awake()
        {
            targetHighlight.gameObject.SetActive(false);
        }

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
            Cursor.SetCursor(IsInteractable ? interactableCursor : null, interactableCursorHotspotOffset, CursorMode.Auto);
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
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void OnDeselected()
        {
            selectionHighlight.gameObject.SetActive(false);
            HidePotentialAttackTargets();
        }

        public void OnSelected()
        {
            selectionHighlight.gameObject.SetActive(true);
            ShowPotentialAttackTargets();
        }

        public void RegisterEdgeVisual(MapNodeVisualController node2, MapEdgeVisualController edgeVisual)
        {
            _edgeVisuals.Add(node2._mapNode, edgeVisual);
        }
        
        /// <summary>
        /// Shows or hides the highlight visuals on this object that identify it as a valid attack target
        /// </summary>
        public void ShowOrHideHighlightsAsPotentialAttackTarget(bool show)
        {
            targetHighlight.gameObject.SetActive(show);
        }

        private void ShowPotentialAttackTargets() => ShowOrHidePotentialAttackTargets(true);

        private void HidePotentialAttackTargets() => ShowOrHidePotentialAttackTargets(false);

        private void ShowOrHidePotentialAttackTargets(bool show)
        {
            foreach (MapNode adjSpace in _edgeVisuals.Keys)
            {
                if (show && adjSpace.CanBeAttackedByAGivenNode(_mapNode))
                {
                    _edgeVisuals[adjSpace].ShowPotentialAttackVisuals(this);
                }
                else
                {
                    _edgeVisuals[adjSpace].HidePotentialAttackVisuals(this);
                }
            }
        }
    }
}

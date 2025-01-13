using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using BattleDataModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleRunner
{
    public abstract class TerritoryVisualControllerBase : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializedDictionary("Adjacent Territory", "Edge Object")]
        [SerializeField] public SerializedDictionary<TerritoryVisualControllerBase, EdgeVisualControllerBase> edges = new();
        
        public BattleRunnerController BattleRunnerController { get; private set; }
        public MapNode Territory { get; private set; }
        
        
        public void Initialize(BattleRunnerController battleRunnerController, MapNode territory)
        {
            BattleRunnerController = battleRunnerController;
            Territory = territory;
            Debug.Log("Territory ID : " + territory.NodeIndex);
            OnInitialize();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsSelected())
            {
                OnPointerEnterWhenDeselectable(eventData);
            }
            else if (IsValidAttacker())
            {
                OnPointerEnterWhenSelectable(eventData);
            }
            else if (IsValidAttackTarget()) 
            {
                OnPointerEnterWhenAttackable(eventData);
            }
            else
            {
                OnPointerEnterWhenUninteractable(eventData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsSelected())
            {
                BattleRunnerController.DeselectTerritory();
            }
            else if (IsValidAttacker())
            {
                BattleRunnerController.SelectTerritory(this);
            }
            else if (IsValidAttackTarget()) 
            {
                BattleRunnerController.ExecuteAttack(this);
            }
        }
        
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnSelect();
        public abstract void OnDeselect();
        
        protected abstract void OnInitialize();
        protected abstract void OnPointerEnterWhenSelectable(PointerEventData eventData);
        protected abstract void OnPointerEnterWhenDeselectable(PointerEventData eventData);
        protected abstract void OnPointerEnterWhenAttackable(PointerEventData eventData);
        protected abstract void OnPointerEnterWhenUninteractable(PointerEventData eventData);
        
        private bool IsSelected()
        {
            return BattleRunnerController.SelectedTerritory == this;
        }
        
        private bool IsValidAttacker()
        {
            bool belongsToActivePlayer = Territory.OwnerPlayerIndex == BattleRunnerController.Battle.ActivePlayer.PlayerIndex;
            bool noTerritoryIsSelected = BattleRunnerController.SelectedTerritory == null;
            bool hasMoreThanOneDie = Territory.NumDice > 1;
            bool hasAdjacentEnemyTerritories = Territory.AdjacentNodes.Any(t => t.OwnerPlayerIndex != Territory.OwnerPlayerIndex);

            bool isValidAttacker = belongsToActivePlayer && noTerritoryIsSelected && hasMoreThanOneDie && hasAdjacentEnemyTerritories;
            return isValidAttacker;
        }

        private bool IsValidAttackTarget()
        {
            bool belongsToActivePlayer = Territory.OwnerPlayerIndex == BattleRunnerController.Battle.ActivePlayer.PlayerIndex;
            bool isAdjacentToSelectedTerritory = BattleRunnerController.SelectedTerritory != null && BattleRunnerController.SelectedTerritory.Territory.AdjacentNodes.Contains(Territory);
            
            bool isValidAttackTarget = !belongsToActivePlayer && isAdjacentToSelectedTerritory;
            return isValidAttackTarget;
        }
    }
}
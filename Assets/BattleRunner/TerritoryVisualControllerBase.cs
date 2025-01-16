using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using BattleDataModel;
using GlobalScripts;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace BattleRunner
{
    public abstract class TerritoryVisualControllerBase : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializedDictionary("Adjacent Territory", "Edge Object")]
        [SerializeField] public SerializedDictionary<TerritoryVisualControllerBase, EdgeVisualControllerBase> edges = new();
        
        public BattleRunnerController BattleRunnerController { get; private set; }
        public MapNode Territory { get; private set; }

        private bool _pointerOver;
        private bool _attacking;
        private bool _beingAttacked;
        public bool HighlightedToShowLargestContiguousGroupOfTerritories {get; set;}
        
        public void Initialize(BattleRunnerController battleRunnerController, MapNode territory)
        {
            BattleRunnerController = battleRunnerController;
            Territory = territory;
            BattleRunnerController.Battle.StartingReinforcementsAllocated += OnStartingReinforcementsAllocated;
            BattleRunnerController.Battle.TurnEnded += OnTurnEnded;
            BattleRunnerController.SelectedTerritoryChanged += OnSelectedTerritoryChanged;
            
            OnInitialize();
        }

        public void UpdateState()
        {
            UpdateInfo();
            if (HighlightedToShowLargestContiguousGroupOfTerritories)
            {
                SetState(State.HighlightedToShowLargestContiguousGroupOfTerritories);
            }
            else if (_attacking)
            {
                SetState(State.Attacking);
            }
            else if (_beingAttacked)
            {
                SetState(State.Defending);
            }
            else if (_pointerOver && IsSelectable())
            {
                SetState(State.HoverSelectable);
            }
            else if (_pointerOver && IsSelected())
            {
                SetState(State.HoverDeselectable);
            }
            else if (_pointerOver && IsValidAttackTarget())
            {
                SetState(State.HoverAttackable);
            }
            else if (IsSelected())
            {
                SetState(State.Selected);
            }
            else if (IsSelectable())
            {
                SetState(State.Selectable);
            }
            else if (IsValidAttackTarget())
            {
                SetState(State.Attackable);
            }
            else
            {
                SetState(State.Normal);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _pointerOver = true;
            UpdateState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerOver = false;
            UpdateState();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsSelected())
            {
                BattleRunnerController.DeselectTerritory();
            }
            else if (IsSelectable())
            {
                BattleRunnerController.SelectTerritory(this);
            }
            else if (IsValidAttackTarget()) 
            {
                BattleRunnerController.ExecuteAttack(BattleRunnerController.SelectedTerritory, this);
            }
        }
        
        /// <summary>
        /// Update ownership and die count visuals
        /// </summary>
        protected abstract void UpdateInfo();

        protected abstract void SetState(State state);
        
        protected abstract void OnInitialize();
        
        private bool IsSelected()
        {
            return BattleRunnerController.SelectedTerritory == this;
        }
        
        private bool IsSelectable()
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
        
        private void OnStartingReinforcementsAllocated(object sender, BattleEvents.StartingReinforcementsAllocatedArgs e)
        {
            UserCueSequencer.EnqueueCueWithNoDelay(UpdateState, nameof(TerritoryVisualControllerBase) + "." + nameof(OnStartingReinforcementsAllocated));
        }
        
        private void OnTurnEnded(object sender, BattleEvents.TurnEndedArgs e)
        {
            UpdateState();
            //UserCueSequencer.EnqueueCueWithNoDelay(UpdateState, nameof(TerritoryVisualControllerBase) + "." + nameof(OnTurnEnded));
        }

        private void OnSelectedTerritoryChanged()
        {
            UpdateState();
            //UserCueSequencer.EnqueueCueWithNoDelay(UpdateState, nameof(TerritoryVisualControllerBase) + "." + nameof(OnSelectedTerritoryChanged));
        }
        
        public enum State
        {
            Normal,
            HoverSelectable,
            HoverDeselectable,
            HoverAttackable,
            Selectable,
            Selected,
            Attackable,
            Attacking,
            Defending,
            HighlightedToShowLargestContiguousGroupOfTerritories,
        }
    }
}
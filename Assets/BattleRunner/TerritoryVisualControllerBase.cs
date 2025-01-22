using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using BattleDataModel;
using BattleDataModel.AiPlayerStrategies;
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
        
        private bool _pointerOver;
        
        public BattleRunnerController BattleRunnerController { get; private set; }
        public MapNode Territory { get; private set; }
        public bool Attacking { get; set; }
        public bool BeingAttacked { get; set; }
        public bool HighlightedToShowLargestContiguousGroupOfTerritories {get; set;}

        protected Battle Battle => BattleRunnerController.Battle;
        
        
        public void Initialize(BattleRunnerController battleRunnerController, MapNode territory)
        {
            BattleRunnerController = battleRunnerController;
            Territory = territory;
            BattleRunnerController.Battle.StartingReinforcementsAllocated += OnStartingReinforcementsAllocated;
            BattleRunnerController.Battle.TurnEnded += OnTurnEnded;
            BattleRunnerController.SelectedTerritoryChanged += OnSelectedTerritoryChanged;
            
            OnInitialize();
        }

        public void UpdateState() => UpdateState(true);
        
        public void UpdateState(bool updateGameData)
        {
            if (updateGameData)
            {
                UpdateGameData();
            }
            
            
            if (HighlightedToShowLargestContiguousGroupOfTerritories)
            {
                SetState(State.HighlightedToShowLargestContiguousGroupOfTerritories);
            }
            else if (Attacking)
            {
                SetState(State.Attacking);
            }
            else if (BeingAttacked)
            {
                SetState(State.Defending);
            }
            else if (!IsLocalHumanPlayersTurn())
            {
                SetState(State.Normal);
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
            //DebugAiStrategyHelpers();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerOver = false;
            if (!UserCueSequencer.CurrentlyProcessingCues)
            {
                UpdateState();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsLocalHumanPlayersTurn())
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
        }
        
        public void OverrideState(State state) => SetState(state);
        
        /// <summary>
        /// Used during the end-of-turn reinforcements visuals to show each die getting assigned to a territory
        /// </summary>
        public abstract void ShowNumDice(int numDice);
        
        /// <summary>
        /// Update ownership and die count visuals
        /// </summary>
        protected abstract void UpdateGameData();

        protected abstract void SetState(State state);
        
        protected abstract void OnInitialize();

        private bool IsLocalHumanPlayersTurn()
        {
            return !BattleRunnerController.Battle.ActivePlayer.IsAiPlayer;
        }
        
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
            UserCueSequencer.EnqueueCueWithNoDelay(UpdateState, nameof(TerritoryVisualControllerBase) + "." + nameof(OnTurnEnded));
        }
        
        private void OnSelectedTerritoryChanged()
        {
            UpdateState();
            //UserCueSequencer.EnqueueCueWithNoDelay(UpdateState, nameof(TerritoryVisualControllerBase) + "." + nameof(OnSelectedTerritoryChanged));
        }
        
        private void DebugAiStrategyHelpers()
        {
            bool canReachWeakTerritoryOfActivePlayer = AiStrategyHelpers.CanReachWeakTerritoryOfTargetPlayer(Territory, Battle.ActivePlayer.PlayerIndex);
            bool capturingTerritoryWouldGrowLargestRegionByTwoOrMore = AiStrategyHelpers.CapturingTerritoryWouldGrowLargestRegionByTwoOrMore(Territory, Battle.Map.GetLargestContiguousGroupOfTerritories(Battle.ActivePlayer.PlayerIndex));
            
            int longestAttackChain = 0;
            foreach (MapNode mapNode in Territory.AdjacentNodes)
            {
                if (mapNode.OwnerPlayerIndex != Territory.OwnerPlayerIndex)
                {
                    int attackChainLength = AiStrategyHelpers.GetAttackChainLength(Territory, mapNode);
                    longestAttackChain = Mathf.Max(longestAttackChain, attackChainLength);
                }
            }
            
            Debug.Log("Longest Attack Chain: " + longestAttackChain);
            Debug.Log("Can Reach Weak Territory of Active Player: " + canReachWeakTerritoryOfActivePlayer);
            Debug.Log("Capturing Territory Would Grow Largest Region by Two or More: " + capturingTerritoryWouldGrowLargestRegionByTwoOrMore);
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
using System;
using BattleDataModel;
using BattleRunner.UI;
using BattleRunner.UI.ReinforcementDicePanel;
using BattleRunner.UI.RollDisplayPanel;
using GlobalScripts;
using Maps;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRunner
{
    public class BattleRunnerController : MonoBehaviour
    {
        public event Action BattleInitialized;
        public event Action BattleStarted;
        public event Action SelectedTerritoryChanged;
        
        [SerializeField] private Canvas mapRoot;
        [SerializeField] private GraphicRaycaster mapCanvasGraphicRaycaster;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private AttackRollsPanelController attackRollsPanel;
        [SerializeField] private ReinforcementDicePanelController reinforcementDicePanel;
        
        private bool _battleStarted;
        
        public GameplayMap GameplayMap {get; private set;}
        public Battle Battle {get; private set;}
        public TerritoryVisualControllerBase SelectedTerritory {get; private set;}
        
        private void Start()
        {
            startGameButton.gameObject.SetActive(true);
            endTurnButton.gameObject.SetActive(false);
            attackRollsPanel.gameObject.SetActive(false);
            
#if UNITY_EDITOR
            BattleLoader.EnsureInitialized();
#endif
            // Instantiate selected map
            GameplayMap = Instantiate(BattleLoader.SelectedMapPrefab, mapRoot.transform);
            GameplayMap.RectTransform.anchoredPosition = Vector2.zero;
            
            // Construct data model battle
            Battle = BattleLoader.ConstructBattle(GameplayMap);
            Battle.RollingAttack += OnRollingAttack;
            Battle.ApplyingReinforcements += OnApplyingReinforcements;
            Battle.AppliedReinforcementDie += OnAppliedReinforcementDie;
            
            // Link nodes to node visuals
            var order = GameplayMap.GetNodeDefinitionsInOrder();
            for (var i = 0; i < order.Length; i++)
            {
                var nodeDefinition = order[i];
                nodeDefinition.GetComponent<TerritoryVisualControllerBase>().Initialize(this, Battle.Map.Nodes[i]);
                Destroy(nodeDefinition);
            }
            
            // Assign territories and initial reinforcements
            Battle.RandomlyAssignTerritories();
            Battle.RandomlyAllocateStartingReinforcements(BattleLoader.StartingReinforcements);
            BattleInitialized?.Invoke();
            
            
            // for testing, auto start. In the future, player will start
            OnClickStartGame();
        }
        
        private void Update()
        {
            mapCanvasGraphicRaycaster.enabled = !UserCueSequencer.CurrentlyProcessingCues && _battleStarted;
        }

        public void SelectTerritory(TerritoryVisualControllerBase territory)
        {
            if (SelectedTerritory != null)
            {
                DeselectTerritory();
            }
            
            SelectedTerritory = territory;
            territory.UpdateState();
            SelectedTerritoryChanged?.Invoke();
        }
        
        public void DeselectTerritory()
        {
            if (SelectedTerritory != null)
            {
                SelectedTerritory.UpdateState();
            }
            SelectedTerritory = null;
            SelectedTerritoryChanged?.Invoke();
        }
        
        public void ExecuteAttack(TerritoryVisualControllerBase attackingTerritory, TerritoryVisualControllerBase targetTerritory)
        {
            SetAllTerritoriesToNormalState();
            attackingTerritory.Attacking = true;
            targetTerritory.BeingAttacked = true;
            attackingTerritory.UpdateState();
            targetTerritory.UpdateState();
            
            Battle.Attack(attackingTerritory.Territory, targetTerritory.Territory);
        }
        
        public void OnClickStartGame()
        {
            startGameButton.gameObject.SetActive(false);
            endTurnButton.gameObject.SetActive(true);
            _battleStarted = true;
            
            BattleStarted?.Invoke();
        }
        
        public void OnClickEndTurn()
        {
            Battle.EndTurn();
        }

        private void SetAllTerritoriesToNormalState()
        {
            foreach (MapNode territory in Battle.Map.Nodes.Values)
            {
                var visualController = GameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                visualController.OverrideState(TerritoryVisualControllerBase.State.Normal);
            }
        }
        
        private void OnRollingAttack(object sender, BattleEvents.RollingAttackArgs e)
        {
            UserCueSequencer.EnqueueCueWithNoDelay(attackRollsPanel.ShowBlank, "Show attack roll display");
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await attackRollsPanel.RunAttackRoll(e.AttackRoll, e.AttackingPlayerId), "show attacker roll");
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await attackRollsPanel.RunDefenseRoll(e.DefenseRoll, e.DefendingPlayerId), "show defender roll");
            
            UserCueSequencer.Wait(.25f);
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                DeselectTerritory();
                attackRollsPanel.Hide();
                
                var attackingTerritoryVisual = GameplayMap.GetTerritoryGameObject(e.AttackingTerritory).GetComponent<TerritoryVisualControllerBase>();
                attackingTerritoryVisual.Attacking = false;
                attackingTerritoryVisual.UpdateState();
                
                var defendingTerritoryVisual = GameplayMap.GetTerritoryGameObject(e.DefendingTerritory).GetComponent<TerritoryVisualControllerBase>();
                defendingTerritoryVisual.BeingAttacked = false;
                defendingTerritoryVisual.UpdateState();
                
            }, "Show Attack Results");
        }
        
        private void OnApplyingReinforcements(object sender, BattleEvents.ApplyingReinforcementsArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() =>
            {
                reinforcementDicePanel.ShowReinforcementDice(e.NumReinforcements, e.PlayerIndex);
            }, nameof(BattleRunnerController) + "." + nameof(OnApplyingReinforcements));
        }
        
        private void OnAppliedReinforcementDie(object sender, BattleEvents.AppliedReinforcementDieArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() =>
            {
                var territoryVisualController = GameplayMap.GetTerritoryGameObject(e.Territory).GetComponent<TerritoryVisualControllerBase>();
                territoryVisualController.ShowNumDice(e.CurrentNumDice);
                reinforcementDicePanel.ShowReinforcementDice(e.NumReinforcementsLeftUnapplied, e.Territory.OwnerPlayerIndex);
            }, nameof(BattleRunnerController) + "." + nameof(OnAppliedReinforcementDie));
        }
    }
}

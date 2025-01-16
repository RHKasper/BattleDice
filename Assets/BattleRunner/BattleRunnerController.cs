using System;
using BattleDataModel;
using BattleRunner.UI;
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
            Debug.Log("ExecuteAttack");
            Battle.Attack(attackingTerritory.Territory, targetTerritory.Territory);
            SetAllTerritoriesToNormalState();
            // todo: move AttackRollsPanelController.OnRolling code here and delete OnRolling event
            
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                DeselectTerritory();
                attackingTerritory.UpdateState();
                targetTerritory.UpdateState();
            }, "Show attack results");
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
                //visualController.OverrideState(TerritoryVisualControllerBase.State.Normal);
            }
        }
    }
}

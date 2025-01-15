using System;
using BattleDataModel;
using BattleRunner.UI;
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
        [SerializeField] private PlayersInfoPanelController playersInfoPanelController;
        
        public GameplayMap GameplayMap {get; private set;}
        public Battle Battle {get; private set;}
        public TerritoryVisualControllerBase SelectedTerritory {get; private set;}
        
        private void Start()
        {
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
            Battle.Attack(attackingTerritory.Territory, targetTerritory.Territory);
            
            UserCueSequencer.EnqueueCueWithDelayAfter(() => Debug.Log("Todo: roll attack and defense dice"), "Roll attack and Defense Dice");
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                attackingTerritory.UpdateState();
                targetTerritory.UpdateState();
            }, "Show attack results");
        }
        
        public void OnClickStartGame()
        {
            startGameButton.gameObject.SetActive(false);
            mapCanvasGraphicRaycaster.enabled = true;
            
            BattleStarted?.Invoke();
        }
    }
}

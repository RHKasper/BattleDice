using System;
using BattleDataModel;
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
        
        public void ExecuteAttack(TerritoryVisualControllerBase targetTerritory)
        {
            Battle.Attack(SelectedTerritory.Territory, targetTerritory.Territory);
            SelectedTerritory.UpdateState();
            targetTerritory.UpdateState();
        }
        
        public void OnClickStartGame()
        {
            startGameButton.gameObject.SetActive(false);
            mapCanvasGraphicRaycaster.enabled = true;
            
            BattleStarted?.Invoke();
        }
    }
}

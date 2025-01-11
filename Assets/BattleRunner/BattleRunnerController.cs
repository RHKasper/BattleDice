using System;
using BattleDataModel;
using GlobalScripts;
using Maps;
using UnityEngine;

namespace BattleRunner
{
    public class BattleRunnerController : MonoBehaviour
    {
        [SerializeField] private Canvas mapRoot;
        
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
            Battle = BattleLoader.ConstructBattle();
            
            // link nodes to node visuals
            var order = GameplayMap.GetNodeDefinitionsInOrder();
            for (var i = 0; i < order.Length; i++)
            {
                var nodeDefinition = order[i];
                nodeDefinition.GetComponent<TerritoryVisualControllerBase>().Initialize(this, Battle.Map.Nodes[i]);
                Destroy(nodeDefinition);
            }

            // Generate Edges?
            
            // wait for player to start game
        }

        public void SelectTerritory(TerritoryVisualControllerBase territory)
        {
            if (SelectedTerritory != null)
            {
                DeselectTerritory();
            }
            
            SelectedTerritory = territory;
            SelectedTerritory.OnSelect();
        }
        
        public void DeselectTerritory()
        {
            if (SelectedTerritory != null)
            {
                SelectedTerritory.OnDeselect();
            }
            SelectedTerritory = null;
        }
        
        public void ExecuteAttack(TerritoryVisualControllerBase territory)
        {
            throw new NotImplementedException();
        }
        
        private void OnClickStartGame()
        {
            // start game
            throw new NotImplementedException();
        }
    }
}

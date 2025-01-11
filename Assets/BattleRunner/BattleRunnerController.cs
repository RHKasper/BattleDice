using System;
using BattleDataModel;
using GlobalScripts;
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
            GameplayMap = Instantiate(BattleLoader.SelectedMap, mapRoot.transform);
            GameplayMap.RectTransform.anchoredPosition = Vector2.zero;

            // Instantiate data model map
            // Instantiate & init data model battle
            // Init map nodes and edges
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

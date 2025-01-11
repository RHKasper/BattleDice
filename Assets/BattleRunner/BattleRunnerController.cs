using System;
using GlobalScripts;
using UnityEngine;

namespace BattleRunner
{
    public class BattleRunnerController : MonoBehaviour
    {
        [SerializeField] private Canvas mapRoot;
        
        public GameplayMap GameplayMap {get; private set;}
        
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
            // Start game?
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlobalScripts
{
    public static class BattleLoader
    {
        private const string GameplayMapsResourcesFolder = "CustomMaps";
        
        public static GameplayMap SelectedMap { get; private set; }
        
        public static void LoadBattle(GameplayMap map)
        {
            SelectedMap = map;
            SceneManager.LoadScene("BattleRunner");
        }

        public static void EnsureInitialized()
        {
            if (SelectedMap == null)
            {
                SelectedMap = GetCustomMaps().First();
            }
        }
        
        public static List<GameplayMap> GetCustomMaps()
        {
            return Resources.LoadAll<GameplayMap>(GameplayMapsResourcesFolder).ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using BattleDataModel;
using BattleDataModel.AiPlayerStrategies;
using Maps;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlobalScripts
{
    public static class BattleLoader
    {
        private const string GameplayMapsResourcesFolder = "CustomMaps";
        
        public static GameplayMap SelectedMapPrefab { get; private set; }
        public static int PlayerCount { get; private set; }
        public static int RandomSeed { get; private set; } = int.MinValue;
        public static int StartingReinforcements { get; private set; } = -1;

        public static void LoadBattle(GameplayMap map)
        {
            SelectedMapPrefab = map;
            SceneManager.LoadScene("BattleRunner");
        }

        public static void EnsureInitialized()
        {
            if (SelectedMapPrefab == null)
            {
                SelectedMapPrefab = GetCustomMaps().First();
            }

            if (PlayerCount == 0)
            {
                PlayerCount = 3;
            }

            if (RandomSeed == int.MinValue)
            {
                RandomSeed = DateTime.Now.Millisecond;
            }

            if (StartingReinforcements == -1)
            {
                StartingReinforcements = 12;
            }
        }

        public static Battle ConstructBattle(GameplayMap mapInstance)
        {
            var players = new List<Player>();
            players.Add(new Player(0, new DefensiveAiStrategy()));
            for (int i = 1; i < PlayerCount; i++)
            {
                players.Add(new Player(i, new AggressiveAiStrategy()));    
            }
            
            Battle b = new Battle(mapInstance.GenerateMapData(), players, RandomSeed);
            return b;
        }
        
        public static List<GameplayMap> GetCustomMaps()
        {
            return Resources.LoadAll<GameplayMap>(GameplayMapsResourcesFolder).ToList();
        }
    }
}

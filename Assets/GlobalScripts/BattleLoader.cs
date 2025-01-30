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
        private const string GameplayScenariosResourcesFolder = "CustomScenarios";
        
        public static GameplayMap SelectedMapPrefab { get; private set; }
        public static List<Player> Players { get; private set; }
        public static int RandomSeed { get; private set; } = int.MinValue;
        public static int StartingReinforcements { get; private set; } = -1;

        public static void LoadBattle(GameplayMap map)
        {
            SelectedMapPrefab = map;

            if (map is GameplayScenario scenario)
            {
                Players = scenario.GetPlayers();
                StartingReinforcements = 0;
            }
            
            SceneManager.LoadScene("BattleRunner");
        }

        public static void EnsureInitialized()
        {
            if (SelectedMapPrefab == null)
            {
                SelectedMapPrefab = GetCustomMaps().First();
            }

            if (Players == null)
            {
                Players = new List<Player> { new(0) };
            
                for (int i = 1; i < 3; i++)
                {
                    Players.Add(new Player(i, i % 2 == 0 ? new AggressiveAiStrategy() : new DefensiveAiStrategy()));    
                }
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
            Battle b = new Battle(mapInstance.GenerateMapData(), Players, RandomSeed);
            return b;
        }
        
        public static List<GameplayMap> GetCustomMaps()
        {
            return Resources.LoadAll<GameplayMap>(GameplayMapsResourcesFolder).ToList();
        }
        
        public static List<GameplayScenario> GetCustomScenarios()
        {
            return Resources.LoadAll<GameplayScenario>(GameplayScenariosResourcesFolder).ToList();
        }
    }
}

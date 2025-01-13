using System;
using System.Collections.Generic;
using System.Linq;
using BattleDataModel;
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
                StartingReinforcements = 5;
            }
        }

        public static Battle ConstructBattle()
        {
            var players = new List<Player>();
            for (int i = 0; i < PlayerCount; i++)
            {
                players.Add(new Player(i));    
            }
            
            Battle b = new Battle(SelectedMapPrefab.GenerateMapData(), players, RandomSeed);
            return b;
        }
        
        public static List<GameplayMap> GetCustomMaps()
        {
            return Resources.LoadAll<GameplayMap>(GameplayMapsResourcesFolder).ToList();
        }
    }
}

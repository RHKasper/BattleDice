#if UNITY_EDITOR

using System;
using System.Collections;
using System.IO;
using System.Linq;
using GlobalScripts;
using GlobalScripts.EditorTools;
using Maps;
using UnityEditor;
using UnityEngine;

namespace EditorTools.MapImageGenerator
{
    public class MapImageGeneratorDirector : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private Canvas canvas;

        [ContextMenu("Generate Images")]
        public void GenerateImages()
        {
            if (!Application.isPlaying)
            {
                throw new Exception("Please enter Play Mode");
            }
            StartCoroutine(CaptureImages());
        }

        private IEnumerator CaptureImages()
        {
            #if UNITY_EDITOR
            yield return CaptureMapImages(false);
            yield return CaptureMapImages(true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif
        }

        private IEnumerator CaptureMapImages(bool scenarios = false)
        {
            var maps = scenarios ? BattleLoader.GetCustomScenarios().Cast<GameplayMap>() : BattleLoader.GetCustomMaps();
            string dirPath = Path.Combine("Assets", "Resources", scenarios ? "ScenarioImages" : "MapImages");
            
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);   
            }
            
            foreach (GameplayMap gameplayMap in maps)
            {
                var assetPath = AssetDatabase.GetAssetPath(gameplayMap);
                string generatedImagePath = Path.Combine(dirPath, Path.GetFileNameWithoutExtension(assetPath) + ".png");
                var instance = Instantiate(gameplayMap, canvas.transform);
                yield return null;
                camera.CaptureImage(1920, 1080, generatedImagePath);
                DestroyImmediate(instance.gameObject);
                
                gameplayMap.SetMapPreviewImage(AssetDatabase.LoadAssetAtPath<Sprite>(generatedImagePath));
                EditorUtility.SetDirty(gameplayMap);
            }
        }
    }
}
#endif

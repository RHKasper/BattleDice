#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobalScripts;
using GlobalScripts.EditorTools;
using GlobalScripts.EditorTools.DiceImageGenerator;
using Maps;
using UnityEditor;
using UnityEngine;

namespace EditorTools.MapImageGenerator
{
    public class MapImageGeneratorDirector : MonoBehaviour
    {
        private const int ImageResolution = 256;

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
            AssetDatabase.Refresh();
            #endif
        }

        private IEnumerator CaptureMapImages(bool scenarios = false)
        {
            var maps = scenarios ? BattleLoader.GetCustomScenarios().Cast<GameplayMap>() : BattleLoader.GetCustomMaps();

            foreach (GameplayMap gameplayMap in maps)
            {
                var assetPath = AssetDatabase.GetAssetPath(gameplayMap);
                var instance = Instantiate(gameplayMap, canvas.transform);
                yield return null;
                camera.CaptureImage(ImageResolution, ImageResolution, Path.Combine("Assets", "Resources", scenarios ? "CustomScenarios" : "CustomMaps", Path.GetFileNameWithoutExtension(assetPath) + ".png"));
                DestroyImmediate(instance);
                yield return null;
            }
        }
    }
}
#endif

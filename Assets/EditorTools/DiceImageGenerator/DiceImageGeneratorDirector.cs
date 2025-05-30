#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

using UnityEditor;

namespace GlobalScripts.EditorTools.DiceImageGenerator
{
    public class DiceImageGeneratorDirector : MonoBehaviour
    {
        private const int ImageResolution = 256;
        
        [Header("Faces")]
        [SerializeField] Camera orthoFacesCamera;
        [SerializeField] private D6MaterialsController face1;
        [SerializeField] private D6MaterialsController face2;
        [SerializeField] private D6MaterialsController face3;
        [SerializeField] private D6MaterialsController face4;
        [SerializeField] private D6MaterialsController face5;
        [SerializeField] private D6MaterialsController face6;

        [Header("3/4 View")] 
        [SerializeField] private Camera threeQuartersCamera;
        [SerializeField] private D6MaterialsController threeQuartersD6;

        [Header("Die Stacks")] 
        [SerializeField] private Camera dieStacksCamera;
        [SerializeField] private D6MaterialsController[] stackDice;

        private D6MaterialsController[] FaceObjects => new[] { face1, face2, face3, face4, face5, face6 };
        
        private void Start()
        {
            orthoFacesCamera.gameObject.SetActive(false);
            foreach (D6MaterialsController faceObject in FaceObjects)
            {
                faceObject.gameObject.SetActive(false);
            }
            
            threeQuartersCamera.gameObject.SetActive(false);
            threeQuartersD6.gameObject.SetActive(false);
            
            dieStacksCamera.gameObject.SetActive(false);
            foreach (D6MaterialsController die in stackDice)
            {
                die.gameObject.SetActive(false);
            }
        }

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
            yield return CaptureDieStackImages();
            yield return CaptureThreeQuartersImages();
            yield return CaptureDieFaceImages();
            AssetDatabase.Refresh();
            #endif
        }

        private IEnumerator CaptureThreeQuartersImages()
        {
            threeQuartersCamera.gameObject.SetActive(true);
            threeQuartersD6.gameObject.SetActive(true);
            
            for (var playerIndex = 0; playerIndex < Constants.Colors.Count; playerIndex++)
            {
                var playerColor = Constants.Colors[playerIndex];
                threeQuartersD6.ApplyBodyColor(playerColor);
                yield return null;
                threeQuartersCamera.CaptureImage(ImageResolution, ImageResolution, Constants.GetThreeQuartersSpriteFilePath(playerIndex));
            }

            threeQuartersCamera.gameObject.SetActive(false);
            threeQuartersD6.gameObject.SetActive(false);
            AssetDatabase.Refresh();
        }
        
        private IEnumerator CaptureDieFaceImages()
        {
            orthoFacesCamera.gameObject.SetActive(true);

            for (var playerIndex = 0; playerIndex < Constants.Colors.Count; playerIndex++)
            {
                var playerColor = Constants.Colors[playerIndex];
                for (int faceIndex = 0; faceIndex < FaceObjects.Length; faceIndex++)
                {
                    FaceObjects[faceIndex].gameObject.SetActive(true);
                    FaceObjects[faceIndex].ApplyBodyColor(playerColor);
                    yield return null;

                    orthoFacesCamera.CaptureImage(ImageResolution, ImageResolution, Constants.GetFaceSpriteFilePath(playerIndex, faceIndex + 1));

                    FaceObjects[faceIndex].gameObject.SetActive(false);
                }
            }

            orthoFacesCamera.gameObject.SetActive(false);
            AssetDatabase.Refresh();
        }

        private IEnumerator CaptureDieStackImages()
        {
            dieStacksCamera.gameObject.SetActive(true);

            for (var playerIndex = 0; playerIndex < Constants.Colors.Count; playerIndex++)
            {
                var playerColor = Constants.Colors[playerIndex];

                for (int i = 0; i < stackDice.Length; i++)
                {
                    for (int j = 0; j < stackDice.Length; j++)
                    {
                        stackDice[j].ApplyBodyColor(playerColor);
                        stackDice[j].gameObject.SetActive(j < i + 1);
                        yield return null;
                        dieStacksCamera.CaptureImage(ImageResolution, ImageResolution, Constants.GetDieStackSpriteFilePath(playerIndex, i + 1));
                    }
                }
            }

            dieStacksCamera.gameObject.SetActive(false);
            foreach (var stackDie in stackDice)
            {
                stackDie.gameObject.SetActive(false);
            }
            
            AssetDatabase.Refresh();
        }
    }
}
#endif

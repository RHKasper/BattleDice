using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        private D6MaterialsController[] FaceObjects => new[] { face1, face2, face3, face4, face5, face6 };

        
        private void Start()
        {
            orthoFacesCamera.gameObject.SetActive(false);
            foreach (D6MaterialsController faceObject in FaceObjects)
            {
                faceObject.gameObject.SetActive(false);
            }
        }

        [ContextMenu("Generate Face Images")]
        public void GenerateFaceImages()
        {
            if (!Application.isPlaying)
            {
                throw new Exception("Please enter Play Mode");
            }
            StartCoroutine(CaptureDieFaceImages());
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

                    CaptureImage(orthoFacesCamera, ImageResolution, ImageResolution, Constants.GetFaceSpriteFilePath(playerIndex, faceIndex + 1));

                    FaceObjects[faceIndex].gameObject.SetActive(false);
                }
            }

            orthoFacesCamera.gameObject.SetActive(false);
            AssetDatabase.Refresh();
        }

        public static void CaptureImage(Camera camera, int width, int height, string filePath)
        {
            // Create a RenderTexture with the desired resolution
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
        
            // Assign the RenderTexture to the camera
            RenderTexture originalTexture = camera.targetTexture;
            camera.targetTexture = renderTexture;

            // Render the camera's output to the RenderTexture
            camera.Render();

            // Create a new Texture2D to store the rendered image
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            // Read the RenderTexture into the Texture2D
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            // Reset the RenderTexture
            RenderTexture.active = null;
            camera.targetTexture = originalTexture;

            // Encode the Texture2D to a PNG
            byte[] pngData = tex.EncodeToPNG();

            // Save the PNG file to the specified path
            File.WriteAllBytes(filePath, pngData);

            // Cleanup
            Destroy(renderTexture);
            Destroy(tex);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalScripts.EditorTools.DiceImageGenerator
{
    public class DiceImageGeneratorDirector : MonoBehaviour
    {
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
            StartCoroutine(CaptureDieFaceImages());
        }

        private IEnumerator CaptureDieFaceImages()
        {
            orthoFacesCamera.gameObject.SetActive(true);
            
            foreach (Color playerColor in Constants.Colors)
            {
                for (int i = 0; i < FaceObjects.Length; i++)
                {
                    FaceObjects[i].gameObject.SetActive(true);
                    FaceObjects[i].ApplyBodyColor(playerColor);
                    yield return null;
                    //capture and save image
                    
                    FaceObjects[i].gameObject.SetActive(false);
                }
            }
            
            orthoFacesCamera.gameObject.SetActive(false);
        }
    }
}
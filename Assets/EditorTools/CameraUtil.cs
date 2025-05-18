using System.IO;
using UnityEngine;

namespace GlobalScripts.EditorTools
{
    public static class CameraUtil 
    {
        public static void CaptureImage(this Camera camera, int width, int height, string filePath)
        {
            // Set the camera's clear flags and background color
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0); // Fully transparent

            // Create a RenderTexture with the desired resolution and support for transparency
            RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

            // Assign the RenderTexture to the camera
            RenderTexture originalTexture = camera.targetTexture;
            camera.targetTexture = renderTexture;

            // Render the camera's output to the RenderTexture
            camera.Render();

            // Create a new Texture2D to store the rendered image
            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGBA32, false);

            // Read the RenderTexture into the Texture2D
            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            // Reset the RenderTexture
            RenderTexture.active = null;
            camera.targetTexture = originalTexture;

            // Encode the Texture2D to a PNG
            byte[] pngData = screenshot.EncodeToPNG();

            // Save the PNG file to the specified path
            File.WriteAllBytes(filePath, pngData);

            // Cleanup
            Object.Destroy(renderTexture);
            Object.Destroy(screenshot);
        }
    }
}

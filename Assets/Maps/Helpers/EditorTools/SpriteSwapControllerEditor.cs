using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Maps.Helpers.EditorTools
{
    [CustomEditor(typeof(SpriteSwapController))]
    [CanEditMultipleObjects]
    public class SpriteSwapControllerEditor : Editor
    {
        private SerializedProperty spriteIndexProp;
        private SerializedProperty spritesProp;
        private SerializedProperty imageProp;

        private void OnEnable()
        {
            spriteIndexProp = serializedObject.FindProperty("spriteIndex");
            spritesProp = serializedObject.FindProperty("sprites");
            imageProp = serializedObject.FindProperty("image");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Show a slider for spriteIndex at the top
            if (spritesProp.arraySize > 0)
            {
                // Get the current value and clamp it for safety
                int currentSpriteIndex = spriteIndexProp.intValue;
                int clampedIndex = Mathf.Clamp(currentSpriteIndex, 0, spritesProp.arraySize - 1);

                // Show slider and get the new value
                int newSpriteIndex = EditorGUILayout.IntSlider("Sprite Index", clampedIndex, 0, spritesProp.arraySize - 1);

                // Check if the index changed
                if (newSpriteIndex != currentSpriteIndex)
                {
                    // Update spriteIndex for all selected objects
                    foreach (Object obj in targets)
                    {
                        SpriteSwapController controller = obj as SpriteSwapController;
                        if (controller != null)
                        {
                            Undo.RecordObject(controller, "Change Sprite Index");
                            controller.spriteIndex = newSpriteIndex;

                            // Update the Image component if it exists
                            if (controller.image != null && controller.sprites != null && newSpriteIndex < controller.sprites.Length)
                            {
                                controller.image.sprite = controller.sprites[newSpriteIndex];
                                EditorUtility.SetDirty(controller.image);
                            }

                            EditorUtility.SetDirty(controller);
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Sprites array is empty or null.", MessageType.Warning);
            }

            // Show the Image reference field
            EditorGUILayout.PropertyField(imageProp);

            // Show the Sprites array with + and - buttons
            EditorGUILayout.PropertyField(spritesProp, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

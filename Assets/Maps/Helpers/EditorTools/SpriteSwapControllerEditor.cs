using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Maps.EditorTools
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
                int newSpriteIndex = Mathf.Clamp(spriteIndexProp.intValue, 0, spritesProp.arraySize - 1);
                newSpriteIndex = EditorGUILayout.IntSlider("Sprite Index", newSpriteIndex, 0, spritesProp.arraySize - 1);

                if (newSpriteIndex != spriteIndexProp.intValue)
                {
                    spriteIndexProp.intValue = newSpriteIndex;

                    // Show the selected sprite in the image component
                    Image image = (Image)imageProp.objectReferenceValue;
                    if (image != null && spritesProp.GetArrayElementAtIndex(spriteIndexProp.intValue) != null)
                    {
                        image.sprite = (Sprite)spritesProp.GetArrayElementAtIndex(spriteIndexProp.intValue).objectReferenceValue;
                        EditorUtility.SetDirty(image); // Mark the image as dirty to update in the editor
                    }

                    EditorUtility.SetDirty(target); // Mark the target object as dirty only when index changes
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

using UnityEditor;
using UnityEngine;

namespace GlobalScripts
{
    [CreateAssetMenu(fileName = "NumberSprites", menuName = "ScriptableObjects/NumberSprites")]
    [FilePath("Assets/Resources/ScriptableObjects/NumberSprites", FilePathAttribute.Location.ProjectFolder)]
    public class NumberSpritesSo : ScriptableSingleton<NumberSpritesSo>
    {
        [SerializeField] private Sprite[] sprites;
        
        public Sprite GetSprite(int number)
        {
            return sprites[number];
        }
    }
}
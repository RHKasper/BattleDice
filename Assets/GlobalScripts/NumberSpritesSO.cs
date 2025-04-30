using RKUnityToolkit.ScriptableObjects;
using UnityEngine;

namespace GlobalScripts
{
    [CreateAssetMenu(fileName = "NumberSprites", menuName = "ScriptableObjects/NumberSprites")]
    [SoResourcesPath(ResourcesPath = "ScriptableObjects/NumberSprites")]
    public class NumberSpritesSo : SoSingleton<NumberSpritesSo>
    {
        [SerializeField] private Sprite[] sprites;
        
        public Sprite GetSprite(int number)
        {
            return sprites[number];
        }
    }
}
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Maps
{
    public class SpriteSwapController : MonoBehaviour
    {
        [SerializeField] public int spriteIndex;
        [SerializeField] public Sprite[] sprites;
        [SerializeField] public Image image;
    }
}

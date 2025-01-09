using UnityEngine;
using UnityEngine.UI;

namespace BattleTest.UI.RollDisplayPanel
{
    public class DieRollUiController : MonoBehaviour
    {
        [SerializeField] private Image pipsImage;
        
        public void ShowPips(Sprite pipsSprite)
        {
            pipsImage.sprite = pipsSprite;
        }
    }
}

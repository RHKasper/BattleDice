using GlobalScripts;
using UnityEngine;
using UnityEngine.UI;

namespace ReusableUi
{
    public class TwoDigitNumberDisplay : MonoBehaviour
    {
        [SerializeField] private Image tensPlace;
        [SerializeField] private Image onesPlace;

        private int _currentValue;
        
        public void ShowNumber(int value)
        {
            Debug.Assert(value is >= 0 and <= 99);
            onesPlace.sprite = NumberSpritesSo.Instance.GetSprite(value % 10);
            tensPlace.sprite = NumberSpritesSo.Instance.GetSprite(value / 10);
            tensPlace.gameObject.SetActive(value / 10 != 0);
            _currentValue = value;
        }

        [ContextMenu("Increment")]
        private void Increment()
        {
            ShowNumber((_currentValue + 1) % 100);
        }
        
        [ContextMenu("Decrement")]
        private void Decrement()
        {
            ShowNumber((_currentValue - 1 + 100) % 100);
        }
    }
}

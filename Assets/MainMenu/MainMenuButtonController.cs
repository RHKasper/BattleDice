using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenu
{
    public class MainMenuButtonController : MonoBehaviour
    {
        [SerializeField] private MainMenuTweeningManager tweeningManager;
        [SerializeField] private MainMenuTweeningManager.ScreenState associatedScreenState;
        
        public void OnToggleValueChanged(bool value)
        {
            if (value)
            {
                tweeningManager.SetDesiredScreenState(associatedScreenState);
            }
            else
            {
                tweeningManager.SetDesiredScreenState(MainMenuTweeningManager.ScreenState.Default);
            }
        }
    }
}
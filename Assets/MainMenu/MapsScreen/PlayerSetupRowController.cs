using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalScripts;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.MapsScreen
{
    public class PlayerSetupRowController : MonoBehaviour
    {
        private const float TweenDuration = .2f;
        
        [SerializeField] private Image numberImage;
        [SerializeField] private Image numberBackground;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private int playerIndex;

        private bool _targetActivationState = true;
        private MotionHandle _latestMotionHandle;
        
        private void Awake()
        {
            numberImage.sprite = NumberSpritesSo.Instance.GetSprite(playerIndex + 1);
            numberBackground.color = Constants.Colors[playerIndex];
            dropdown.options = new List<TMP_Dropdown.OptionData>
            {
                new("Strategy 1"),
                new("Strategy 2")
            };
            dropdown.value = 0;
        }
        

        public async Task SetActiveWithTweening(bool desiredActiveState)
        {
            if (_targetActivationState == desiredActiveState)
            {
                // if target state is correct, do nothing
            }
            else if(desiredActiveState)
            {
                _latestMotionHandle.TryComplete();
                
                _targetActivationState = true;
                gameObject.SetActive(true);
                _latestMotionHandle = LSequence.Create().Join(LMotion.Create(0f, 1f, TweenDuration).WithEase(Ease.OutSine).BindToLocalScaleX(transform)).Run();
                await _latestMotionHandle.ToAwaitable(); 
            }
            else
            {
                _latestMotionHandle.TryComplete();
                
                _targetActivationState = false;
                _latestMotionHandle = LSequence.Create().Join(LMotion.Create(1f, 0f, TweenDuration).WithEase(Ease.OutSine).BindToLocalScaleX(transform)).Run();
                await _latestMotionHandle.ToAwaitable();
                gameObject.SetActive(false);
            }
        }
    }
}

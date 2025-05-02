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
        [SerializeField] private Image numberImage;
        [SerializeField] private Image numberBackground;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private int playerIndex;
        
        private const float TweenDuration = .2f;
        
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
        

        public async Task SetActiveWithTweening(bool active)
        {
            if (active && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                await LSequence.Create()
                    .Join(LMotion.Create(0f, 1f, TweenDuration).WithEase(Ease.OutSine).BindToLocalScaleX(transform))
                    .Run().ToAwaitable();
            }
            else if (!active && gameObject.activeSelf)
            {
                await LSequence.Create()
                    .Join(LMotion.Create(1f, 0f, TweenDuration).WithEase(Ease.OutSine).BindToLocalScaleX(transform))
                    .Run().ToAwaitable();
                gameObject.SetActive(false);
            }
        }
    }
}

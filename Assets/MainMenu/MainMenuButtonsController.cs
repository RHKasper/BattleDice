using System;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuButtonsController : MonoBehaviour
    {
        [SerializeField] private float tweenDuration = .5f;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform tweenedRect;

        private Vector2 _origAnchorMin;
        private Vector2 _origAnchorMax;
        private Vector2 _origAnchoredPos;

        private MotionHandle _handle;

        private bool _isTweened = false;
        
        void Start()
        {
            _origAnchorMin = rectTransform.anchorMin;
            _origAnchorMax = rectTransform.anchorMax;
            _origAnchoredPos = rectTransform.anchoredPosition;
        }

        private void Update()
        {
            if (!_handle.IsPlaying() && Input.anyKey)
            {
                if (_isTweened)
                {
                    _handle = LMotion.Create(tweenedRect.anchorMin, _origAnchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(rectTransform);
                    LMotion.Create(tweenedRect.anchorMax, _origAnchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(rectTransform);
                    LMotion.Create(tweenedRect.anchoredPosition, _origAnchoredPos, tweenDuration).WithEase(Ease.OutSine).BindToAnchoredPosition(rectTransform);
                    _isTweened = false;
                }
                else
                {
                    _handle = LMotion.Create(_origAnchorMin, tweenedRect.anchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(rectTransform);
                    LMotion.Create(_origAnchorMax, tweenedRect.anchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(rectTransform);
                    LMotion.Create(_origAnchoredPos, tweenedRect.anchoredPosition, tweenDuration).WithEase(Ease.OutSine).BindToAnchoredPosition(rectTransform);
                    _isTweened = true;
                }
            }
        }
    }
}

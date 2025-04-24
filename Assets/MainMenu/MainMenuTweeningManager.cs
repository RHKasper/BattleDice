using System;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuTweeningManager : MonoBehaviour
    {
        [SerializeField] private RectTransform menuButtonsTweenedRect;
        [SerializeField] private RectTransform menuScreenInitialTweenRect;
        [SerializeField] private RectTransform menuScreenPartiallyExpandedTweenRect;
        
        [SerializeField] private float tweenDuration = .5f;
        [SerializeField] private float tweenedSpacing = -20f;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private MapsPanelController mapsPanel;
        

        private Vector2 _origAnchorMin;
        private Vector2 _origAnchorMax;
        private Vector2 _origAnchoredPos;
        private float _origSpacing;
        
        private ScreenState _currentTargetState = ScreenState.Default;
        private ScreenState _desiredState = ScreenState.Default;

        private Awaitable _lastTween;
        
        void Start()
        {
            _origAnchorMin = rectTransform.anchorMin;
            _origAnchorMax = rectTransform.anchorMax;
            _origAnchoredPos = rectTransform.anchoredPosition;
            _origSpacing = verticalLayoutGroup.spacing;
        }

        private void Update()
        {
            if (_lastTween == null || _lastTween.IsCompleted)
            {
                switch (_currentTargetState)
                {
                    case ScreenState.Default:
                        if (_desiredState != ScreenState.Default)
                        {
                            _lastTween = TweenButtons();
                        }
                        break;
                    case ScreenState.TempButtonsTweened:
                        if (_desiredState == ScreenState.Default)
                        {
                            _lastTween = UnTweenButtons();
                        }
                        break;
                    case ScreenState.Maps:
                    case ScreenState.Scenarios:
                        if (_desiredState == ScreenState.Default)
                        {
                            
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }    
            }
        }

        public void SetDesiredScreenState(ScreenState desiredState)
        {
            _desiredState = desiredState;
        }

        private async Awaitable TweenButtons()
        {
            _currentTargetState = ScreenState.TempButtonsTweened;
            await LSequence.Create().
                Join(LMotion.Create(_origAnchorMin, menuButtonsTweenedRect.anchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(rectTransform)).
                Join(LMotion.Create(_origAnchorMax, menuButtonsTweenedRect.anchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(rectTransform)).
                Join(LMotion.Create(_origAnchoredPos, menuButtonsTweenedRect.anchoredPosition, tweenDuration).WithEase(Ease.OutSine).BindToAnchoredPosition(rectTransform)).
                Join(LMotion.Create(_origSpacing, tweenedSpacing, tweenDuration).WithEase(Ease.OutSine).BindToSpacing(verticalLayoutGroup)).
                Run().ToAwaitable();
        }

        private async Awaitable UnTweenButtons()
        {
            _currentTargetState = ScreenState.Default;
            await LSequence.Create().
                Join(LMotion.Create(menuButtonsTweenedRect.anchorMin, _origAnchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(rectTransform)).
                Join(LMotion.Create(menuButtonsTweenedRect.anchorMax, _origAnchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(rectTransform)).
                Join(LMotion.Create(menuButtonsTweenedRect.anchoredPosition, _origAnchoredPos, tweenDuration).WithEase(Ease.OutSine).BindToAnchoredPosition(rectTransform)).
                Join(LMotion.Create(verticalLayoutGroup.spacing, _origSpacing, tweenDuration).WithEase(Ease.OutSine).BindToSpacing(verticalLayoutGroup)).
                Run().ToAwaitable();
        }

        private async Awaitable ShowScreen(ScreenState screen)
        {
            Debug.Assert(screen != ScreenState.Default && screen != ScreenState.TempButtonsTweened);
            await LSequence.Create().
                Join(LMotion.Create(menuButtonsTweenedRect.anchorMin, _origAnchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(rectTransform)).
                Join(LMotion.Create(menuButtonsTweenedRect.anchorMax, _origAnchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(rectTransform)).
                Join(LMotion.Create(menuButtonsTweenedRect.anchoredPosition, _origAnchoredPos, tweenDuration).WithEase(Ease.OutSine).BindToAnchoredPosition(rectTransform)).
                Join(LMotion.Create(verticalLayoutGroup.spacing, _origSpacing, tweenDuration).WithEase(Ease.OutSine).BindToSpacing(verticalLayoutGroup)).
                Run().ToAwaitable();
        }

        private async Awaitable HideScreen(ScreenState screen)
        {
            Debug.Assert(screen != ScreenState.Default && screen != ScreenState.TempButtonsTweened);
        }
        
        
        
        public enum ScreenState
        {
            Default, TempButtonsTweened, Maps, Scenarios
        }
    }
}

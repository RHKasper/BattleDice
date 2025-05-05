using System;
using System.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MainMenu.MapsScreen;
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
        [SerializeField] private RectTransform menuScreenFullyExpandedTweenRect;
        
        [SerializeField] private float tweenDuration = .5f;
        [SerializeField] private float tweenedSpacing = -20f;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private MapsScreenController mapsScreen;
        [SerializeField] private MapsScreenController scenariosScreen;
        

        private Vector2 _origAnchorMin;
        private Vector2 _origAnchorMax;
        private Vector2 _origAnchoredPos;
        private float _origSpacing;
        
        private ScreenState _currentTargetState = ScreenState.Default;
        private ScreenState _desiredState = ScreenState.Maps;

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
                        switch (_desiredState)
                        {
                            case ScreenState.Default:
                                _lastTween = UnTweenButtons();
                                break;
                            case ScreenState.TempButtonsTweened:
                                break;
                            case ScreenState.Maps:
                            case ScreenState.Scenarios:
                                _lastTween = ShowScreen(_desiredState);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case ScreenState.Maps:
                    case ScreenState.Scenarios:
                        if (_desiredState != _currentTargetState)
                        {
                            _lastTween = HideScreen(_currentTargetState);
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
            _currentTargetState = screen;
            RectTransform screenRectTransform = GetScreen(screen);
            screenRectTransform.gameObject.SetActive(true);

            await CreateRectTransformTween(menuScreenInitialTweenRect, menuScreenPartiallyExpandedTweenRect, screenRectTransform).Run().ToAwaitable();
            await CreateRectTransformTween(menuScreenPartiallyExpandedTweenRect, menuScreenFullyExpandedTweenRect, screenRectTransform).Run().ToAwaitable();
        }

        private async Awaitable HideScreen(ScreenState screen)
        {
            Debug.Assert(screen != ScreenState.Default && screen != ScreenState.TempButtonsTweened);
            _currentTargetState = ScreenState.TempButtonsTweened;
            RectTransform screenRectTransform = GetScreen(screen);
            
            await CreateRectTransformTween(menuScreenFullyExpandedTweenRect, menuScreenPartiallyExpandedTweenRect, screenRectTransform).Run().ToAwaitable();
            await CreateRectTransformTween(menuScreenPartiallyExpandedTweenRect, menuScreenInitialTweenRect, screenRectTransform).Run().ToAwaitable();
            
            screenRectTransform.gameObject.SetActive(false);
        }

        private RectTransform GetScreen(ScreenState screen)
        {
            Debug.Assert(screen != ScreenState.Default && screen != ScreenState.TempButtonsTweened);
            return screen switch
            {
                ScreenState.Maps => mapsScreen.GetComponent<RectTransform>(),
                ScreenState.Scenarios => scenariosScreen.GetComponent<RectTransform>(),
                _ => throw new Exception("Unknown screen " + screen)
            };
        }

        private MotionSequenceBuilder CreateRectTransformTween(RectTransform start, RectTransform end, RectTransform target)
        {
            return LSequence.Create().
                Join(LMotion.Create(start.anchorMin, end.anchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(target)).
                Join(LMotion.Create(start.anchorMax, end.anchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(target)).
                Join(LMotion.Create(start.offsetMin, end.offsetMin, tweenDuration).WithEase(Ease.OutSine).BindToOffsetMin(target)).
                Join(LMotion.Create(start.offsetMax, end.offsetMax, tweenDuration).WithEase(Ease.OutSine).BindToOffsetMax(target)).
                Join(LMotion.Create(start.localScale, end.localScale, tweenDuration).WithEase(Ease.OutSine).BindToLocalScale(target));
        }
        
        public enum ScreenState
        {
            Default, TempButtonsTweened, Maps, Scenarios
        }
    }
}

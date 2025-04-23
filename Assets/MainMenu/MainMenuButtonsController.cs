using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuButtonsController : MonoBehaviour
    {
        [SerializeField] private float tweenDuration = .5f;
        [SerializeField] private float tweenedSpacing = -20f;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform tweenedRect;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

        private Vector2 _origAnchorMin;
        private Vector2 _origAnchorMax;
        private Vector2 _origAnchoredPos;
        private float _origSpacing;
        
        private MotionHandle _handle;
        private TweenState _currentTweenState = TweenState.UnTweened;
        private TweenState _desiredTweenState = TweenState.UnTweened;
        
        void Start()
        {
            _origAnchorMin = rectTransform.anchorMin;
            _origAnchorMax = rectTransform.anchorMax;
            _origAnchoredPos = rectTransform.anchoredPosition;
            _origSpacing = verticalLayoutGroup.spacing;
        }

        private void Update()
        {
            if (!_handle.IsPlaying() && _desiredTweenState != _currentTweenState)
            {
                if (_desiredTweenState == TweenState.UnTweened)
                {
                    ExecuteUnTween();
                }
                else
                {
                    ExecuteTween();
                }
            }
        }
        
        public void SetDesiredTweenState(TweenState desiredState)
        {
            _desiredTweenState = desiredState;
        }

        public void OnButtonToggled(bool value)
        {
            SetDesiredTweenState(value ? TweenState.Tweened : TweenState.UnTweened);
        }
        
        private void ExecuteTween()
        {
            _handle = LMotion.Create(_origAnchorMin, tweenedRect.anchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(rectTransform);
            LMotion.Create(_origAnchorMax, tweenedRect.anchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(rectTransform);
            LMotion.Create(_origAnchoredPos, tweenedRect.anchoredPosition, tweenDuration).WithEase(Ease.OutSine).BindToAnchoredPosition(rectTransform);
            LMotion.Create(_origSpacing, tweenedSpacing, tweenDuration).WithEase(Ease.OutSine).BindToSpacing(verticalLayoutGroup);
            _currentTweenState = TweenState.Tweened;
        }

        public void ExecuteUnTween()
        {
            _handle = LMotion.Create(tweenedRect.anchorMin, _origAnchorMin, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMin(rectTransform);
            LMotion.Create(tweenedRect.anchorMax, _origAnchorMax, tweenDuration).WithEase(Ease.OutSine).BindToAnchorMax(rectTransform);
            LMotion.Create(tweenedRect.anchoredPosition, _origAnchoredPos, tweenDuration).WithEase(Ease.OutSine).BindToAnchoredPosition(rectTransform);
            LMotion.Create(verticalLayoutGroup.spacing, _origSpacing, tweenDuration).WithEase(Ease.OutSine).BindToSpacing(verticalLayoutGroup);
            _currentTweenState = TweenState.UnTweened;
        }
        
        public enum TweenState
        {
            Tweened, UnTweened
        }
    }
}

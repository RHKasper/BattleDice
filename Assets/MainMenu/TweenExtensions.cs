using System;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public static class TweenExtensions
    {
        public static MotionHandle BindToSpacing<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, HorizontalOrVerticalLayoutGroup layoutGroup)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            if (layoutGroup == null) throw new ArgumentNullException(nameof(layoutGroup));
            
            return builder.Bind(layoutGroup, static (x, target) =>
            {
                target.spacing = x;
            });
        }
        
        public static MotionHandle BindToOffsetMin<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, RectTransform rectTransform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));

            return builder.Bind(rectTransform, static (x, target) =>
            {
                target.offsetMin = x;
            });
        }
        
        public static MotionHandle BindToOffsetMax<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, RectTransform rectTransform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));

            return builder.Bind(rectTransform, static (x, target) =>
            {
                target.offsetMax = x;
            });
        }
    }
}
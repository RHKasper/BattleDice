using System;
using LitMotion;
using UnityEditor.PackageManager;
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
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace BattleTest.Scripts
{
    public static class PlayerColors
    {
        public static readonly List<Color> Colors = new()
        {
            new Color(.7f, .1f, .1f),
            new Color(.1f, .7f, .7f),
            new Color(.1f, .7f, .1f),
            new Color(.7f, .7f, .1f),
            new Color(.7f, .7f, .7f),
            new Color(.9f, .1f, .9f),
            new Color(.5f, .4f, .8f),
            new Color(.1f, .4f, .2f),
            new Color(1f, .4f, .2f),
            new Color(1f, .7f, .0f),
        };
    }
}
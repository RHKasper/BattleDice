using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GlobalScripts
{
    public static class Constants
    {
        public static readonly List<Color> Colors = new()
        {
            new Color(31/255f, 120/255f, 180/255f),
            new Color(178/255f, 223/255f, 138/255f),
            new Color(51/255f, 160/255f, 44/255f),
            new Color(251/255f, 154/255f, 153/255f),
            new Color(227/255f, 26/255f, 28/255f),
            new Color(253/255f, 191/255f, 111/255f),
            new Color(255/255f, 127/255f, 0/255f),
            new Color(166/255f, 206/255f, 227/255f),
            new Color(202/255f, 178/255f, 214/255f),
            new Color(106/255f, 61/255f, 154/255f),
            new Color(255/255f, 255/255f, 153/255f),
            new Color(177/255f, 89/255f, 40/255f)
        };
        
        
        public static string OutputDirectoryRoot => Path.Combine(Application.dataPath, "Resources", "Dice");
        public static string FacesOutputDirectory => Path.Combine(OutputDirectoryRoot, "Faces");
        public static string ThreeQuartersViewOutputDirectory => Path.Combine(OutputDirectoryRoot, "ThreeQuarters");
        public static string DieStacksOutputDirectory => Path.Combine(OutputDirectoryRoot, "Stacks");

        public static string GetFaceSpriteFilePath(int playerIndex, int dieValue)
        {
            return Path.Combine(FacesOutputDirectory, $"p{playerIndex}_{dieValue}.png");
        }

        public static string GetThreeQuartersSpriteFilePath(int playerIndex)
        {
            return Path.Combine(ThreeQuartersViewOutputDirectory, $"p{playerIndex}.png");
        }
        
        public static string GetDieStackSpriteFilePath(int playerIndex, int numDice)
        {
            return Path.Combine(DieStacksOutputDirectory, $"p{playerIndex}_{numDice}.png");
        }
    }
}
using JetBrains.Annotations;

namespace BattleDataModel
{
    public class Player
    {
        public int PlayerIndex { get; private set; }
        public bool Eliminated { get; internal set; }
        [CanBeNull] public AiPlayerStrategyBase AiStrategy { get; }
        public bool IsAiPlayer => AiStrategy != null;

        public Player(int playerIndex, [CanBeNull] AiPlayerStrategyBase aiStrategy = null)
        {
            PlayerIndex = playerIndex;
            Eliminated = false;
            AiStrategy = aiStrategy;
        }
    }
}
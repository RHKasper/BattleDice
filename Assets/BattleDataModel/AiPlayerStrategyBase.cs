namespace BattleDataModel
{
    public abstract class AiPlayerStrategyBase
    {
        public abstract void PlayTurn(Battle battle, Player player);
    }
}
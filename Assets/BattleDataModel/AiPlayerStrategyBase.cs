using System.Threading.Tasks;

namespace BattleDataModel
{
    public abstract class AiPlayerStrategyBase
    {
        public abstract void PlayNextMove(Battle battle, Player player);
    }
}
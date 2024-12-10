namespace BattleDataModel
{
    public class Player
    {
        public int PlayerIndex { get; private set; }
        public bool Eliminated { get; internal set; }

        public Player(int playerIndex)
        {
            PlayerIndex = playerIndex;
            Eliminated = false;
        }
    }
}
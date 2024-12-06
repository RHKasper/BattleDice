namespace BattleDataModel
{
    public class Player
    {
        public int PlayerID { get; private set; }

        public Player(int playerID)
        {
            PlayerID = playerID;
        }
    }
}
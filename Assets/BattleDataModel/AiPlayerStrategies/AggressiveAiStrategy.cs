using System;
using UnityEngine;

namespace BattleDataModel.AiPlayerStrategies
{
    public class AggressiveAiStrategy : AiPlayerStrategyBase
    {
        public override void PlayTurn(Battle battle, Player player)
        {
            bool done = false;
            
            while (!done)
            {
                // todo: do stuff :D
                done = true;
            }
            
            Debug.Log("Ending AI Turn");
            battle.EndTurn();
        }
    }
}
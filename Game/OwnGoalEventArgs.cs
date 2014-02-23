using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace LD10.Game
{
    public class OwnGoalEventArgs : EventArgs
    {
        PlayerIndex playerIndex;

        public OwnGoalEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public PlayerIndex PlayerIndex
        {
            get
            {
                return playerIndex;
            }
        }
    }
}

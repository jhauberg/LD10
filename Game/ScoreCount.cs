using System;
using System.Collections.Generic;

using Game.Foundation;
using Microsoft.Xna.Framework;

namespace LD10.Game
{
    public class ScoreCount : Behavior
    {
        Dictionary<PlayerIndex, int> score = new Dictionary<PlayerIndex, int>();
        Dictionary<PlayerIndex, int> tally = new Dictionary<PlayerIndex, int>();

        public ScoreCount()
        {
            score.Add(PlayerIndex.One, 0);
            score.Add(PlayerIndex.Two, 0);

            tally.Add(PlayerIndex.One, 0);
            tally.Add(PlayerIndex.Two, 0);
        }

        public void IncreaseScore(PlayerIndex playerIndex)
        {
            score[playerIndex]++;
            
            if (score[playerIndex] > 2) {
                tally[playerIndex]++;

                score[PlayerIndex.One] = 0;
                score[PlayerIndex.Two] = 0;
            }
        }

        public int GetTally(PlayerIndex playerIndex)
        {
            return tally[playerIndex];
        }

        public int GetMatchScore(PlayerIndex playerIndex)
        {
            return score[playerIndex];
        }

        public int this[PlayerIndex playerIndex]
        {
            get
            {
                return GetMatchScore(playerIndex);
            }
        }
    }
}

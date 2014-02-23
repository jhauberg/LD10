using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Game.Foundation;

namespace LD10.Game
{
    public class GoalRepresentation : Behavior
    {
        [BehaviorDependency]
        Transform transform = null;

        Vector3 extents = new Vector3(1.5f, 2, 3);

        PlayerIndex playerIndex;

        public GoalRepresentation()
            : this(PlayerIndex.One) { }

        public GoalRepresentation(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public Vector3 Extents
        {
            get
            {
                return extents;
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                Vector3 min = -extents / 2;
                Vector3 max = extents / 2;

                return new BoundingBox(
                    Vector3.Transform(min, transform.World),
                    Vector3.Transform(max, transform.World));
            }
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

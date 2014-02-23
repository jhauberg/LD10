using System;

using Game.Foundation;

namespace LD10.Game
{
    public class PlayingField : Behavior
    {
        float width, height, depth;

        public PlayingField()
            : this(10, 0.25f, 5) { }

        public PlayingField(float width, float height, float depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public float Width
        {
            get
            {
                return width;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
        }

        public float Depth
        {
            get
            {
                return depth;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Game.Foundation;

namespace LD10.Game
{
    public class Transform : Behavior
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Matrix Rotation = Matrix.Identity;

        public Matrix World
        {
            get
            {
                return
                    Rotation *
                    Matrix.CreateScale(Scale) *
                    Matrix.CreateTranslation(Position);
            }
        }
    }
}

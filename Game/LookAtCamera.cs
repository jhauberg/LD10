using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace LD10.Game
{
    public class LookAtCamera : Camera
    {
        Transform target;

        public LookAtCamera()
            : base() { }

        public override Matrix View
        {
            get
            {
                return
                    Matrix.CreateLookAt(
                    transform.Position,
                    target != null ?
                        target.Position :
                        Vector3.Zero,
                    Vector3.Up);
            }
        }

        public Transform Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value) {
                    target = value;
                }
            }
        }
    }
}

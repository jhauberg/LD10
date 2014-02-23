using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Game.Foundation;

namespace LD10.Game
{
    public class BallController : Behavior
    {
        [BehaviorDependency]
        Transform transform = null;

        [BehaviorDependency(Group = "Playing Field")]
        PlayingField field = null;

        // hack, todo: remove
        public Vector3 velocity;

        float moveSpeed = 4;
        float friction = 0.98f;

        float a;

        public override void Update(TimeSpan elapsed)
        {
            // clamp to x,z
            velocity.Y = 0;

            transform.Position += velocity;
            velocity *= friction;

            a += (velocity.X + velocity.Z) * 1.5f;

            transform.Rotation = Matrix.CreateFromYawPitchRoll(a, a / 2, a / 4);

            float w = field.Width / 2;
            float d = field.Depth / 2;

            if (transform.Position.X > w) {
                transform.Position.X = w;
                velocity = Vector3.Reflect(velocity, Vector3.Left);
            }
            if (transform.Position.X < -w) {
                transform.Position.X = -w;
                velocity = Vector3.Reflect(velocity, Vector3.Right); ;
            }

            if (transform.Position.Z > d) {
                transform.Position.Z = d;
                velocity = Vector3.Reflect(velocity, Vector3.Forward); ;
            }
            if (transform.Position.Z < -d) {
                transform.Position.Z = -d;
                velocity = Vector3.Reflect(velocity, Vector3.Backward); ;
            }
        }

        public void Reset()
        {
            velocity = Vector3.Zero;

            float offset = 0.5f;

            transform.Position = Vector3.Zero;
            transform.Position.Y = field.Group.Get<Transform>().Position.Y + offset;
        }

        public void Push(Vector3 direction, TimeSpan elapsed)
        {
            velocity += (direction * moveSpeed) * (float)elapsed.TotalSeconds;
        }
    }
}

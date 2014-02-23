using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

namespace LD10.Game
{
    public class BallRepresentation : Behavior
    {
        /*
        [BehaviorDependency]
        BallVisual visual = null;
        */
        [BehaviorDependency]
        Transform transform = null;

        Model ball;

        public override void Initialize()
        {
            base.Initialize();

            // no biggie loading it again as its cached, this is a better solution than depending on the visual behavior
            ball = GameContainer.Content.Load<Model>("Content\\Models\\Ball");
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                // i prefer having absolute boundaries
                //return ball.Meshes[0].BoundingSphere;
                return new BoundingSphere(transform.Position, ball.Meshes[0].BoundingSphere.Radius);
            }
        }
    }
}

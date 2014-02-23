using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

using LD10.Game.Springs;

namespace LD10.Game
{
    public class PlayerTailRepresentation : Behavior
    {
        [BehaviorDependency]
        SpringSkeleton skeleton = null;

        Model tail;

        public override void Initialize()
        {
            base.Initialize();

            // no biggie loading it again as its cached, this is a better solution than depending on the visual behavior
            tail = GameContainer.Content.Load<Model>("Content\\Models\\Player_Tail");
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                return new BoundingSphere(skeleton.Nodes[skeleton.Nodes.Count - 1].Position, tail.Meshes[0].BoundingSphere.Radius);
            }
        }
    }
}

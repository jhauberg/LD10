using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Game.Foundation;

namespace LD10.Game
{
    public class CameraController : Behavior
    {
        /*
        [BehaviorDependency]
        LookAtCamera lookAt = null;
        */
        [BehaviorDependency]
        Transform transform = null;

        // hacky, assuming player 1 is the first group registered
        // could just loop through all groups registered under "Player", but grabbing the references at start is best performance wise
        [BehaviorDependency(Group = "Player", Index = 0)]
        PlayerTailRepresentation player1Tail = null;
        [BehaviorDependency(Group = "Player", Index = 1)]
        PlayerTailRepresentation player2Tail = null;
        
        [BehaviorDependency(Group = "Ball")]
        Transform ballTransform = null;
        
        bool manual = false;

        BoundingSphere sphere;

        public override void Start()
        {
            base.Start();

            // well that sucked.
            //lookAt.Target = ballTransform;
        }

        public override void Update(TimeSpan elapsed)
        {
            if (!manual) {
                sphere = BoundingSphere.CreateFromPoints(
                    new Vector3[] {
                    player1Tail.BoundingSphere.Center,
                    player2Tail.BoundingSphere.Center, 
                    ballTransform.Position});

                //transform.Position.X = ballTransform.Position.X;
                transform.Position.Y = sphere.Radius * 2.5f;
            } else {
                float rX = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;
                float rY = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y;

                transform.Position.Y += -rY;
            }

            if (transform.Position.Y < 13) {
                transform.Position.Y = 13;
            }
        }
        /*
        public override void Draw()
        {
            if (sphere != null) {
                GameContainer.VectorRenderer.DrawBoundingSphere(sphere);
            }
        }
        */
        public bool Manual
        {
            get
            {
                return manual;
            }
            set
            {
                manual = value;
            }
        }
    }
}

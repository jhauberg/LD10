using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

using LD10.Game.Springs;

namespace LD10.Game
{
    public class GoalVisual : DrawableBehavior
    {
        [BehaviorDependency]
        SpringSkeleton skeleton = null;

        [BehaviorDependency]
        GoalRepresentation goal = null;

        [BehaviorDependency(Group = "Camera")]
        LookAtCamera camera = null;//Camera camera = null;

        public override void Initialize()
        {
            base.Initialize();

            skeleton.Enabled = true;

            skeleton.Coefficient = 0.3f;
            skeleton.Elasticity = 0.01f;
            skeleton.EnergyLoss = 0.86f;

            float width = 2;
            float height = 2;
            float depth = 3;
            float w = width / 2;
            float h = height / 2;
            float d = depth / 2;

            Vector3 offset = Vector3.Zero;//transform.Position;

            PlayingField field = GameContainer.Scene.Coordinator.Select("Playing Field")[0].Get<PlayingField>();


            if (goal.PlayerIndex == PlayerIndex.One) {
                offset = new Vector3(-(field.Width / 2) - (goal.Extents.X / 2) + 1.5f, 1, 0);
            } else if (goal.PlayerIndex == PlayerIndex.Two) {
                // hacky, i've messed something up with positioning, so the right goal seems further away..
                offset = new Vector3((field.Width / 2) + (goal.Extents.X / 2) - 3.5f, 1, 0);
            }

            // goal

            SpringNode a1 = null;

            // :>
            if (goal.PlayerIndex == PlayerIndex.One) {
                a1 = new SpringNode(offset + new Vector3(-w, -h, d));
            } else if (goal.PlayerIndex == PlayerIndex.Two) {
                a1 = new SpringNode(offset + new Vector3(w * 3, -h, d));
            }

            SpringNode a2 = new SpringNode(offset + new Vector3(w, -h, d));
            SpringNode a3 = new SpringNode(offset + new Vector3(w, h, d));

            SpringNode b1 = null;

            // :>
            if (goal.PlayerIndex == PlayerIndex.One) {
                b1 = new SpringNode(offset + new Vector3(-w, -h, -d));
            } else if (goal.PlayerIndex == PlayerIndex.Two) {
                b1 = new SpringNode(offset + new Vector3(w * 3, -h, -d));
            }

            SpringNode b2 = new SpringNode(offset + new Vector3(w, -h, -d));
            SpringNode b3 = new SpringNode(offset + new Vector3(w, h, -d));

            SpringNodeAnchor a1Anchor = new SpringNodeAnchor(a1);
            SpringNodeAnchor a2Anchor = new SpringNodeAnchor(a2);
            SpringNodeAnchor a3Anchor = new SpringNodeAnchor(a3);

            SpringNodeAnchor b1Anchor = new SpringNodeAnchor(b1);
            SpringNodeAnchor b2Anchor = new SpringNodeAnchor(b2);
            SpringNodeAnchor b3Anchor = new SpringNodeAnchor(b3);

            a1.Neighbors.Add(a2, Vector3.Distance(a1.Position, a2.Position));
            a1.Neighbors.Add(a3, Vector3.Distance(a1.Position, a3.Position));
            a1.Neighbors.Add(b1, Vector3.Distance(a1.Position, b1.Position));

            a2.Neighbors.Add(a1, Vector3.Distance(a2.Position, a1.Position));
            a2.Neighbors.Add(a3, Vector3.Distance(a2.Position, a3.Position));

            a3.Neighbors.Add(a1, Vector3.Distance(a3.Position, a1.Position));
            a3.Neighbors.Add(a2, Vector3.Distance(a3.Position, a2.Position));
            a3.Neighbors.Add(b3, Vector3.Distance(a3.Position, b3.Position));

            b1.Neighbors.Add(b2, Vector3.Distance(b1.Position, b2.Position));
            b1.Neighbors.Add(b3, Vector3.Distance(b1.Position, b3.Position));
            b1.Neighbors.Add(a1, Vector3.Distance(b1.Position, a1.Position));

            b2.Neighbors.Add(b1, Vector3.Distance(b2.Position, b1.Position));
            b2.Neighbors.Add(b3, Vector3.Distance(b2.Position, b3.Position));

            b3.Neighbors.Add(b1, Vector3.Distance(b3.Position, b1.Position));
            b3.Neighbors.Add(b2, Vector3.Distance(b3.Position, b2.Position));
            b3.Neighbors.Add(a3, Vector3.Distance(b3.Position, a3.Position));

            // net
            float dist = 0.5f;

            SpringNode na1 = new SpringNode(new Vector3(0, 0, 0));
            SpringNode na2 = new SpringNode(new Vector3(dist, 0, 0));
            SpringNode na3 = new SpringNode(new Vector3(dist * 2, 0, 0));
            SpringNode na4 = new SpringNode(new Vector3(dist * 3, 0, 0));
            SpringNode na5 = new SpringNode(new Vector3(0, dist, 0));
            SpringNode na6 = new SpringNode(new Vector3(dist, dist, 0));
            SpringNode na7 = new SpringNode(new Vector3(dist * 2, dist, 0));
            SpringNode na8 = new SpringNode(new Vector3(dist * 3, dist, 0));

            na1.Neighbors.Add(na2, dist);
            na1.Neighbors.Add(na5, dist);

            na2.Neighbors.Add(na1, dist);
            na2.Neighbors.Add(na6, dist);
            na2.Neighbors.Add(na3, dist);

            na3.Neighbors.Add(na2, dist);
            na3.Neighbors.Add(na4, dist);
            na3.Neighbors.Add(na7, dist);

            na4.Neighbors.Add(na3, dist);
            na4.Neighbors.Add(na8, dist);

            na5.Neighbors.Add(na1, dist);
            na5.Neighbors.Add(na6, dist);

            na6.Neighbors.Add(na5, dist);
            na6.Neighbors.Add(na2, dist);
            na6.Neighbors.Add(na7, dist);

            na7.Neighbors.Add(na6, dist);
            na7.Neighbors.Add(na3, dist);
            na7.Neighbors.Add(na8, dist);

            na8.Neighbors.Add(na7, dist);
            na8.Neighbors.Add(na4, dist);

            a1.Neighbors.Add(na4, dist);
            na4.Neighbors.Add(a1, dist);

            b1.Neighbors.Add(na1, dist);
            na1.Neighbors.Add(b1, dist);

            a3.Neighbors.Add(na8, dist);
            na8.Neighbors.Add(a3, dist);

            b3.Neighbors.Add(na5, dist);
            na5.Neighbors.Add(b3, dist);

            skeleton.Nodes.AddRange(
                new SpringNode[] { 
                    a1, a2, a3, b1, b2, b3,
                    na1, na2, na3, na4, na5, na6, na7, na8 });
            skeleton.Anchors.AddRange(new SpringNodeAnchor[] { a1Anchor, a2Anchor, a3Anchor, b1Anchor, b2Anchor, b3Anchor });
        }

        public override void Draw()
        {
            GameContainer.VectorRenderer.SetViewProjMatrix(camera.View * camera.Projection);
            GameContainer.VectorRenderer.SetColor(
                (goal.PlayerIndex == PlayerIndex.One ?
                    Color.Red :
                    Color.Blue));

            foreach (SpringNode node in skeleton.Nodes) {
                foreach (SpringNode neighbor in node.Neighbors.Keys) {
                    GameContainer.VectorRenderer.SetWorldMatrix(Matrix.Identity);
                    GameContainer.VectorRenderer.DrawLine(node.Position, neighbor.Position);
                }
            }
            /*
            GameContainer.VectorRenderer.SetWorldMatrix(Matrix.Identity);
            GameContainer.VectorRenderer.SetViewProjMatrix(camera.View * camera.Projection);
            GameContainer.VectorRenderer.DrawBoundingBox(goal.BoundingBox);*/
        }
    }
}

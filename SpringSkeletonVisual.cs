using System;
using System.Collections.Generic;
using System.Text;
using Game.Foundation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD10
{
    /*
    public class SpringSkeletonVisual : DrawableBehavior
    {
        [BehaviorDependency]
        SpringSkeleton skeleton = null;

        [BehaviorDependency(Group = "Camera")]
        Camera camera = null;

        Vector3 nodeExtents = new Vector3(0.1f, 0.1f, 0.1f);

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw()
        {
            float scale = 1;
            
            foreach (SpringNode node in skeleton.Nodes) {
                BoundingBox bounds = new BoundingBox(
                    node.Position - (nodeExtents * scale),
                    node.Position + (nodeExtents * scale));

                GameContainer.VectorRenderer.SetViewProjMatrix(camera.View * camera.Projection);
                GameContainer.VectorRenderer.SetWorldMatrix(Matrix.Identity);

                GameContainer.VectorRenderer.SetColor(
                    SpringNode.IsAnchor(node, skeleton) ?
                        Color.Red :
                        Color.White);

                GameContainer.VectorRenderer.DrawBoundingBox(bounds);

                if (scale > 0.1f) {
                    scale -= 0.1f;
                }
            }
        }
    }*/
}

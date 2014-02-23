using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

using LD10.Game.Springs;

namespace LD10.Game
{
    public class PlayerVisual : DrawableBehavior
    {
        [BehaviorDependency]
        SpringSkeleton skeleton = null;

        [BehaviorDependency]
        PlayerController controller = null;

        [BehaviorDependency(Group = "Camera")]
        LookAtCamera camera = null;//Camera camera = null;

        Effect velvety;
        
        EffectParameter fxViewInverted;
        EffectParameter fxWorldInvertedTranspose;
        EffectParameter fxWorld;
        EffectParameter fxWvp;
        EffectParameter fxSubColor;
        EffectParameter fxDiffColor;
        EffectParameter fxSpecColor;

        Model part, tail;

        Matrix[] tailTransforms;

        float a;

        public override void Initialize()
        {
            base.Initialize();

            part = GameContainer.Content.Load<Model>("Content\\Models\\Player_Part");
            tail = GameContainer.Content.Load<Model>("Content\\Models\\Player_Tail");

            tailTransforms = new Matrix[tail.Bones.Count];
            tail.CopyAbsoluteBoneTransformsTo(tailTransforms);

            velvety = GameContainer.Content.Load<Effect>("Content\\Effects\\Velvety");
          
            fxWorldInvertedTranspose = velvety.Parameters["WorldITXf"];
            fxWvp = velvety.Parameters["WvpXf"];
            fxWorld = velvety.Parameters["WorldXf"];
            fxViewInverted = velvety.Parameters["ViewIXf"];
            fxSubColor = velvety.Parameters["SubColor"];
            fxDiffColor = velvety.Parameters["DiffColor"];
            fxSpecColor = velvety.Parameters["SpecColor"];

            for (int i = 0; i < part.Meshes.Count; i++) {
                for (int j = 0; j < part.Meshes[i].MeshParts.Count; j++) {
                    part.Meshes[i].MeshParts[j].Effect = velvety;
                }
            }

            for (int i = 0; i < tail.Meshes.Count; i++) {
                for (int j = 0; j < tail.Meshes[i].MeshParts.Count; j++) {
                    tail.Meshes[i].MeshParts[j].Effect = velvety;
                }
            }
        }

        Matrix rotation = Matrix.Identity;

        public override void Update(TimeSpan elapsed)
        {
            SpringNode tail = skeleton.Nodes[skeleton.Nodes.Count - 1];

            a += tail.Velocity.X + tail.Velocity.Z;

            rotation = Matrix.CreateFromYawPitchRoll(a, a / 2, a / 4);
        }

        public override void Draw()
        {
            GameContainer.Graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            float scale = 1;

            velvety.CurrentTechnique = velvety.Techniques["Simple"];

            fxViewInverted.SetValue(Matrix.Invert(camera.View));

            fxSubColor.SetValue(
                (controller.PlayerIndex == PlayerIndex.One ? 
                    new Vector3(1, 0, 0) : // red
                    new Vector3(0, 0, 1))); // blue

            fxDiffColor.SetValue(
                (controller.PlayerIndex == PlayerIndex.One ? 
                    new Vector3(0.75f, 0.511f, 0.503f) : 
                    new Vector3(0.215f, 0.477f, 0.75f)));
            fxSpecColor.SetValue(new Vector3(0.5f, 0.5f, 0.5f));

            for (int i = 0; i < skeleton.Nodes.Count; i++) {
                if (i == skeleton.Nodes.Count - 1) {
                    // draw tail
                    foreach (ModelMesh mesh in tail.Meshes) {
                        Matrix world = tailTransforms[mesh.ParentBone.Index] * rotation * Matrix.CreateTranslation(skeleton.Nodes[i].Position);
                        
                        fxWorldInvertedTranspose.SetValueTranspose(Matrix.Invert(world));
                        fxWorld.SetValue(world);
                        fxWvp.SetValue(world * camera.View * camera.Projection);
                        
                        velvety.CommitChanges();

                        mesh.Draw();
                    }
                } else {
                    // draw part
                    foreach (ModelMesh mesh in part.Meshes) {
                        Matrix world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(skeleton.Nodes[i].Position);

                        fxWorldInvertedTranspose.SetValueTranspose(Matrix.Invert(world));
                        fxWorld.SetValue(world);
                        fxWvp.SetValue(world * camera.View * camera.Projection);
                        
                        velvety.CommitChanges();

                        mesh.Draw();
                    }

                    if (scale > 0.1f) {
                        scale -= 0.1f;
                    }
                }
            }
        }
    }
}

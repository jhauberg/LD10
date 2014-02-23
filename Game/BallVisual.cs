using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

namespace LD10.Game
{
    public class BallVisual : DrawableBehavior
    {
        [BehaviorDependency]
        Transform transform = null;

        [BehaviorDependency(Group = "Camera")]
        LookAtCamera camera = null;//Camera camera = null;

        Model ball;

        Effect velvety;

        EffectParameter fxViewInverted;
        EffectParameter fxWorldInvertedTranspose;
        EffectParameter fxWorld;
        EffectParameter fxWvp;
        EffectParameter fxTexture;
        EffectParameter fxSubColor;
        EffectParameter fxDiffColor;
        EffectParameter fxSpecColor;

        Texture2D rubbery;

        public override void Initialize()
        {
            base.Initialize();

            ball = GameContainer.Content.Load<Model>("Content\\Models\\Ball");

            rubbery = GameContainer.Content.Load<Texture2D>("Content\\Textures\\Rubbery");

            velvety = GameContainer.Content.Load<Effect>("Content\\Effects\\Velvety");

            fxWorldInvertedTranspose = velvety.Parameters["WorldITXf"];
            fxWvp = velvety.Parameters["WvpXf"];
            fxWorld = velvety.Parameters["WorldXf"];
            fxViewInverted = velvety.Parameters["ViewIXf"];
            fxTexture = velvety.Parameters["ColorTexture"];

            fxSubColor = velvety.Parameters["SubColor"];
            fxDiffColor = velvety.Parameters["DiffColor"];
            fxSpecColor = velvety.Parameters["SpecColor"];

            for (int i = 0; i < ball.Meshes.Count; i++) {
                for (int j = 0; j < ball.Meshes[i].MeshParts.Count; j++) {
                    ball.Meshes[i].MeshParts[j].Effect = velvety;
                }
            }
        }

        public override void Draw()
        {
            velvety.CurrentTechnique = velvety.Techniques["Textured"];

            foreach (ModelMesh mesh in ball.Meshes) {
                Matrix world = transform.World;

                fxTexture.SetValue(rubbery);
                fxViewInverted.SetValue(Matrix.Invert(camera.View));
                fxWorldInvertedTranspose.SetValueTranspose(Matrix.Invert(world));
                fxWorld.SetValue(world);
                fxWvp.SetValue(world * camera.View * camera.Projection);

                fxSubColor.SetValue(Color.Gold.ToVector3());
                fxDiffColor.SetValue(Color.Gold.ToVector3());

                velvety.CommitChanges();

                mesh.Draw();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Game.Foundation;

namespace LD10.Game
{
    public class PlayingFieldVisual : DrawableBehavior
    {
        [BehaviorDependency]
        PlayingField field = null;

        [BehaviorDependency]
        Transform transform = null;

        [BehaviorDependency(Group = "Camera")]
        LookAtCamera camera = null;//Camera camera = null;

        Effect lambert;

        EffectParameter fxTexture;
        EffectParameter fxViewInverted;
        EffectParameter fxWorldInvertedTranspose;
        EffectParameter fxWvp;
        EffectParameter fxWorld;

        Texture2D grassy;

        VertexDeclaration vertexDecl;
        VertexBuffer vb;
        IndexBuffer ib;

        public override void Initialize()
        {
            base.Initialize();

            grassy = GameContainer.Content.Load<Texture2D>("Content\\Textures\\Grassy2");

            lambert = GameContainer.Content.Load<Effect>("Content\\Effects\\LambertTextured");

            fxTexture = lambert.Parameters["ColorTexture"];
            fxViewInverted = lambert.Parameters["ViewIXf"];
            fxWorld = lambert.Parameters["WorldXf"];
            fxWorldInvertedTranspose = lambert.Parameters["WorldITXf"];
            fxWvp = lambert.Parameters["WvpXf"];

            vertexDecl = new VertexDeclaration(GameContainer.Graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            float w = field.Width / 2;
            float h = field.Height / 2;
            float d = field.Depth / 2;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[8]
            {
                new VertexPositionNormalTexture(new Vector3(-w, h, d), Vector3.Up, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(w, h, d), Vector3.Up, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(w, -h, d), Vector3.Down, new Vector2()),
                new VertexPositionNormalTexture(new Vector3(-w, -h, d), Vector3.Down, new Vector2()),

                new VertexPositionNormalTexture(new Vector3(-w, h, -d), Vector3.Up, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(w, h, -d), Vector3.Up, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(w, -h, -d), Vector3.Down, new Vector2()),
                new VertexPositionNormalTexture(new Vector3(-w, -h, -d), Vector3.Down, new Vector2())
            };

            short[] indices = new short[36]
            {
                // front face
                0, 1, 2,
                2, 3, 0,

                // back face
                4, 5, 6,
                6, 7, 4,

                // left face
                0, 4, 7,
                7, 3, 0,

                // right face
                1, 5, 6,
                6, 2, 1,

                // top face
                0, 4, 5,
                5, 1, 0,

                // bottom face
                3, 7, 6,
                6, 2, 3
            };

            ib = new IndexBuffer(GameContainer.Graphics.GraphicsDevice, VertexPositionNormalTexture.SizeInBytes * indices.Length, ResourceUsage.None, IndexElementSize.SixteenBits);
            ib.SetData(indices);

            vb = new VertexBuffer(GameContainer.Graphics.GraphicsDevice, VertexPositionNormalTexture.SizeInBytes * vertices.Length, ResourceUsage.None);
            vb.SetData(vertices);
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            lambert.CurrentTechnique = lambert.Techniques["Main"];

            fxTexture.SetValue(grassy);
            fxViewInverted.SetValue(Matrix.Invert(camera.View));

            fxWorldInvertedTranspose.SetValueTranspose(Matrix.Invert(transform.World));
            fxWvp.SetValue(transform.World * camera.View * camera.Projection);
            fxWorld.SetValue(transform.World);

            lambert.CommitChanges();

            lambert.Begin();
            foreach (EffectPass pass in lambert.CurrentTechnique.Passes) {
                pass.Begin();

                device.VertexDeclaration = vertexDecl;
                device.Indices = ib;
                device.Vertices[0].SetSource(vb, 0, VertexPositionNormalTexture.SizeInBytes);

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);

                pass.End();
            }
            lambert.End();
        }
    }
}

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

namespace LD10.Game
{
    public class BarrierVisual : DrawableBehavior
    {
        [BehaviorDependency]
        Transform transform = null;

        [BehaviorDependency(Group = "Camera")]
        LookAtCamera camera = null;//Camera camera = null;

        VertexDeclaration vertexDecl;
        VertexBuffer vb;
        IndexBuffer ib;

        Texture2D advert;

        BasicEffect effect;

        float width = 1;
        float height = 1;

        public Texture2D Advertisement
        {
            get
            {
                return advert;
            }
            set
            {
                if (advert != value) {
                    advert = value;
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            effect = new BasicEffect(GameContainer.Graphics.GraphicsDevice, null);

            vertexDecl = new VertexDeclaration(GameContainer.Graphics.GraphicsDevice, VertexPositionColorTexture.VertexElements);

            float w = width / 2;
            float h = height / 2;

            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4]
            {
                new VertexPositionColorTexture(new Vector3(-w, -h, 0), Color.Black, textureLowerLeft),
                new VertexPositionColorTexture(new Vector3(-w, h, 0), Color.Black, textureUpperLeft),
                new VertexPositionColorTexture(new Vector3(w, -h, 0), Color.Black, textureLowerRight),
                new VertexPositionColorTexture(new Vector3(w, h, 0), Color.Black, textureUpperRight)
            };

            short[] indices = new short[6]
            {
                0, 1, 2,
                2, 1, 3
            };

            vb = new VertexBuffer(GameContainer.Graphics.GraphicsDevice, VertexPositionColorTexture.SizeInBytes * vertices.Length, ResourceUsage.None);
            vb.SetData(vertices);

            ib = new IndexBuffer(GameContainer.Graphics.GraphicsDevice, VertexPositionColorTexture.SizeInBytes * indices.Length, ResourceUsage.None, IndexElementSize.SixteenBits);
            ib.SetData(indices);
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Alpha = 1;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Begin();

                device.VertexDeclaration = vertexDecl;
                device.Indices = ib;
                device.Vertices[0].SetSource(vb, 0, VertexPositionColorTexture.SizeInBytes);

                effect.World = transform.World;
                effect.View = camera.View;
                effect.Projection = camera.Projection;

                effect.Texture = advert;

                effect.CommitChanges();

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

                pass.End();
            }
            effect.End();
        }
    }
}

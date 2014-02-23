using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

namespace LD10.Game
{
    public class ScoreVisual : DrawableBehavior
    {
        [BehaviorDependency]
        ScoreCount counter = null;

        [BehaviorDependency(Group = "Camera")]
        LookAtCamera camera = null;//Camera camera = null;

        [BehaviorDependency(Group = "Playing Field")]
        PlayingField field = null;

        VertexDeclaration vertexDecl;
        VertexBuffer vb;
        IndexBuffer ib;

        // this is such a damn crappy solution ;)
        Texture2D score0, score1, score2;//, score3;

        BasicEffect effect;

        float width = 1;
        float depth = 1;

        public override void Initialize()
        {
            base.Initialize();

            effect = new BasicEffect(GameContainer.Graphics.GraphicsDevice, null);

            score0 = GameContainer.Content.Load<Texture2D>("Content\\Textures\\Score_0");
            score1 = GameContainer.Content.Load<Texture2D>("Content\\Textures\\Score_1");
            score2 = GameContainer.Content.Load<Texture2D>("Content\\Textures\\Score_2");
            //score3 = GameContainer.Content.Load<Texture2D>("Content\\Textures\\Score_3");

            vertexDecl = new VertexDeclaration(GameContainer.Graphics.GraphicsDevice, VertexPositionColorTexture.VertexElements);

            float w = width / 2;
            float d = depth / 2;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4]
            {
                new VertexPositionColorTexture(new Vector3(-w, 0, d), Color.Black, new Vector2(0, 1)),
                new VertexPositionColorTexture(new Vector3(w, 0, d), Color.Black, new Vector2(1, 1)),
                new VertexPositionColorTexture(new Vector3(w, 0, -d), Color.Black, new Vector2(1, 0)),
                new VertexPositionColorTexture(new Vector3(-w, 0, -d), Color.Black, new Vector2(0, 0))
            };

            short[] indices = new short[6]
            {
                0, 1, 2,
                0, 3, 2
            };

            vb = new VertexBuffer(GameContainer.Graphics.GraphicsDevice, VertexPositionColorTexture.SizeInBytes * vertices.Length, ResourceUsage.None);
            vb.SetData(vertices);

            ib = new IndexBuffer(GameContainer.Graphics.GraphicsDevice, VertexPositionColorTexture.SizeInBytes * indices.Length, ResourceUsage.None, IndexElementSize.SixteenBits);
            ib.SetData(indices);
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Alpha = 1;

            float scoreOffset = 1.85f;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Begin();

                device.VertexDeclaration = vertexDecl;
                device.Indices = ib;
                device.Vertices[0].SetSource(vb, 0, VertexPositionColorTexture.SizeInBytes);

                switch (counter[PlayerIndex.One]) {
                    default: effect.Texture = score0; break;
                    case 0: effect.Texture = score0; break;
                    case 1: effect.Texture = score1; break;
                    case 2: effect.Texture = score2; break;
                    //case 3: effect.Texture = score3; break;
                }

                effect.World = Matrix.CreateTranslation(new Vector3(-(field.Width / 2) + (width / 2) + scoreOffset, 0.2f, 0));
                effect.View = camera.View;
                effect.Projection = camera.Projection;
                effect.CommitChanges();

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

                switch (counter[PlayerIndex.Two]) {
                    default: effect.Texture = score0; break;
                    case 0: effect.Texture = score0; break;
                    case 1: effect.Texture = score1; break;
                    case 2: effect.Texture = score2; break;
                    //case 3: effect.Texture = score3; break;
                }

                effect.World = Matrix.CreateTranslation(new Vector3((field.Width / 2) - (width / 2) - scoreOffset, 0.2f, 0));
                effect.View = camera.View;
                effect.Projection = camera.Projection;
                effect.CommitChanges();

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

                pass.End();
            }
            effect.End();

            device.RenderState.AlphaBlendEnable = false;
        }
    }
}

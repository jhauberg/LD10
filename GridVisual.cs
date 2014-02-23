using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Game.Foundation;

namespace LD10
{
    /*
    public class GridVisual : DrawableBehavior
    {
        [BehaviorDependency]
        Transform transform = null;
        [BehaviorDependency]
        Grid grid = null;

        [BehaviorDependency(Group = "Camera")]
        Camera camera = null;

        VertexDeclaration vertexDeclaration;
        VertexBuffer vb;
        int vertexCount;

        Effect effect;

        EffectParameter effectWvp;
        EffectParameter effectAlpha;

        public override void Initialize()
        {
            base.Initialize();

            effect = GameContainer.Content.Load<Effect>("Content\\Effects\\Minimal");
            effect.CurrentTechnique = effect.Techniques["Minimal"];

            effectWvp = effect.Parameters["WorldViewProj"];
            effectAlpha = effect.Parameters["Alpha"];

            grid.Changed += new EventHandler(grid_Changed);

            BuildVertices();
        }

        void grid_Changed(object sender, EventArgs e)
        {
            BuildVertices();
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            device.RenderState.CullMode = CullMode.None;

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            effectWvp.SetValue(
                transform.World * 
                camera.View * 
                camera.Projection);

            effectAlpha.SetValue(0.5f);

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Begin();
                device.VertexDeclaration = vertexDeclaration;
                device.Vertices[0].SetSource(vb, 0, VertexPositionColor.SizeInBytes);
                device.DrawPrimitives(PrimitiveType.LineList, 0, vertexCount / 2);

                pass.End();
            }
            effect.End();

            device.RenderState.AlphaBlendEnable = false;
        }

        private void BuildVertices()
        {
            int columns = grid.Columns;
            int rows = grid.Rows;

            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            vertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);

            VertexPositionColor[] columnVertices = new VertexPositionColor[(columns * 2) + 2];

            float right = ((float)columns / 2) * grid.TileSize; // amount of unit steps to take, to reach the side (right)
            float back = ((float)rows / 2) * grid.TileSize;     // amount of unit steps to take, to reach the back

            float offset = 0.0f;

            for (int i = 0; i < columnVertices.Length - 1; i += 2) {
                columnVertices[i] = new VertexPositionColor(new Vector3(-right + offset, 0, back), Color.White);
                columnVertices[i + 1] = new VertexPositionColor(new Vector3(-right + offset, 0, -back), Color.White);

                offset += grid.TileSize;
            }

            VertexPositionColor[] rowVertices = new VertexPositionColor[(rows * 2) + 2];

            offset = 0.0f;

            for (int i = 0; i < rowVertices.Length - 1; i += 2) {
                rowVertices[i] = new VertexPositionColor(new Vector3(right, 0, -back + offset), Color.White);
                rowVertices[i + 1] = new VertexPositionColor(new Vector3(-right, 0, -back + offset), Color.White);

                offset += grid.TileSize;
            }

            VertexPositionColor[] combinedVertices = new VertexPositionColor[columnVertices.Length + rowVertices.Length];

            Array.Copy(columnVertices, 0, combinedVertices, 0, columnVertices.Length);
            Array.Copy(rowVertices, 0, combinedVertices, columnVertices.Length, rowVertices.Length);

            vertexCount = combinedVertices.Length;

            vb = new VertexBuffer(device, VertexPositionColor.SizeInBytes * combinedVertices.Length, ResourceUsage.None);
            vb.SetData<VertexPositionColor>(combinedVertices);
        }
    }*/
}

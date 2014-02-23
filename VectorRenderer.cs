using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LD10
{
    // Class written by someone in the xna community - i nabbed it from an article on ziggyware.com
    public class VectorRenderComponent : DrawableGameComponent
    {

        #region Private Members

        private ContentManager content = null;

        private Effect effect = null;
        private Matrix viewProjMatrix = Matrix.Identity;
        private Matrix worldMatrix = Matrix.Identity;

        private EffectParameter m_paramVP = null;
        private EffectParameter m_paramWorld = null;

        private VertexDeclaration vertexDecl = null;

        private const int maxPoints = 8;
        private VertexPositionColor[] vertices =
                    new VertexPositionColor[maxPoints];

        private const int maxIndices = 24;
        private short[] indices = new short[maxIndices];

        private int currentPass = -1;

        private string vectorRendererEffectName = "";

        #endregion

        #region Constructor
        public VectorRenderComponent(Microsoft.Xna.Framework.Game g, ContentManager content, string vectorRendererEffectName)
            : base(g)
        {
            this.content = content;
            this.vectorRendererEffectName = vectorRendererEffectName;
        }
        #endregion


        #region LoadGraphicsContent
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent) {

                IGraphicsDeviceService graphicsService =
                    (IGraphicsDeviceService)base.Game.Services.GetService(
                                                typeof(IGraphicsDeviceService));

                GraphicsDevice device = graphicsService.GraphicsDevice;


                effect = content.Load<Effect>(vectorRendererEffectName);

                m_paramVP = effect.Parameters["mVP"];
                m_paramWorld = effect.Parameters["mWorld"];

                vertexDecl = new VertexDeclaration(
                                    device,
                                    VertexPositionColor.VertexElements);

                SetColor(Color.White);

                #region Setup Indices
                // setup box indices
                indices[0] = 0;
                indices[1] = 1;
                indices[2] = 1;
                indices[3] = 2;
                indices[4] = 2;
                indices[5] = 3;
                indices[6] = 3;
                indices[7] = 0;

                indices[8] = 4;
                indices[9] = 5;
                indices[10] = 5;
                indices[11] = 6;
                indices[12] = 6;
                indices[13] = 7;
                indices[14] = 7;
                indices[15] = 4;

                indices[16] = 0;
                indices[17] = 4;
                indices[18] = 1;
                indices[19] = 5;
                indices[20] = 2;
                indices[21] = 6;
                indices[22] = 3;
                indices[23] = 7;
                #endregion
            }
        }
        #endregion

        #region UnloadGraphicsContent

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent) {
                vertexDecl.Dispose();
                vertexDecl = null;
            }
        }

        #endregion


        #region void SetWorldMatrix(Matrix pworldMatrix)
        public void SetWorldMatrix(Matrix pworldMatrix)
        {
            worldMatrix = pworldMatrix;
        }
        #endregion


        #region void SetViewProjMatrix(Matrix pviewProjMatrix)
        public void SetViewProjMatrix(Matrix pviewProjMatrix)
        {
            viewProjMatrix = pviewProjMatrix;
        }
        #endregion

        #region void SetColor
        public void SetColor(Color color)
        {
            for (int i = 0; i < maxPoints; ++i) {
                vertices[i].Color = color;
            }
        }

        public void SetColor(Vector3 color)
        {
            for (int i = 0; i < maxPoints; ++i) {
                vertices[i].Color = new Color(color);
            }
        }
        #endregion


        #region void DrawLine(Vector3 p0, Vector3 p1)
        public void DrawLine(Vector3 p0, Vector3 p1)
        {
            IGraphicsDeviceService graphicsService =
                    (IGraphicsDeviceService)base.Game.Services.GetService(
                                                typeof(IGraphicsDeviceService));

            GraphicsDevice device = graphicsService.GraphicsDevice;


            vertices[0].Position = p0;
            vertices[1].Position = p1;

            Predraw(0);
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                                                           vertices, 0, 1);
            Postdraw();
        }
        #endregion

        #region void DrawLine(int x0, int y0, int x1, int y1)
        public void DrawLine(int x0, int y0, int x1, int y1)
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)
                    base.Game.Services.GetService(typeof(IGraphicsDeviceService));

            GraphicsDevice device = graphicsService.GraphicsDevice;

            vertices[0].Position = new Vector3(
                -1.0f + 2.0f * x0 / device.PresentationParameters.BackBufferWidth,
                1.0f - 2.0f * y0 / device.PresentationParameters.BackBufferHeight, 0);

            vertices[1].Position = new Vector3(
                -1.0f + 2.0f * x1 / device.PresentationParameters.BackBufferWidth,
                1.0f - 2.0f * y1 / device.PresentationParameters.BackBufferHeight, 0);

            Predraw(1);
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                                                           vertices, 0, 1);
            Postdraw();
        }
        #endregion

        #region void DrawLine2D(Vector3 p0, Vector3 p1)
        public void DrawLine2D(Vector3 p0, Vector3 p1)
        {
            DrawLine((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y);

        }
        #endregion

        #region void DrawBoundingBox(BoundingBox box)
        public void DrawBoundingBox(BoundingBox box)
        {
            IGraphicsDeviceService graphicsService =
                    (IGraphicsDeviceService)base.Game.Services.GetService(
                                                    typeof(IGraphicsDeviceService));

            GraphicsDevice device = graphicsService.GraphicsDevice;


            vertices[0].Position = new Vector3(box.Min.X, box.Min.Y, box.Min.Z);
            vertices[1].Position = new Vector3(box.Max.X, box.Min.Y, box.Min.Z);
            vertices[2].Position = new Vector3(box.Max.X, box.Min.Y, box.Max.Z);
            vertices[3].Position = new Vector3(box.Min.X, box.Min.Y, box.Max.Z);

            vertices[4].Position = new Vector3(box.Min.X, box.Max.Y, box.Min.Z);
            vertices[5].Position = new Vector3(box.Max.X, box.Max.Y, box.Min.Z);
            vertices[6].Position = new Vector3(box.Max.X, box.Max.Y, box.Max.Z);
            vertices[7].Position = new Vector3(box.Min.X, box.Max.Y, box.Max.Z);

            Predraw(0);
            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList, vertices,
                0,
                8,
                indices,
                0,
                12);
            Postdraw();
        }
        #endregion

        #region void DrawBoundingSphere(BoundingSphere sphere)
        public void DrawBoundingSphere(BoundingSphere sphere)
        {
            const int numCircleSegments = 12;

            float step = 2.0f * (float)Math.PI / numCircleSegments;

            for (int i = 0; i < numCircleSegments; ++i) {
                float u0 = (float)Math.Cos(step * i) * sphere.Radius;
                float v0 = (float)Math.Sin(step * i) * sphere.Radius;
                float u1 = (float)Math.Cos(step * (i + 1)) * sphere.Radius;
                float v1 = (float)Math.Sin(step * (i + 1)) * sphere.Radius;

                // xy
                DrawLine(new Vector3(u0, v0, 0) + sphere.Center,
                         new Vector3(u1, v1, 0) + sphere.Center);

                // xz
                DrawLine(new Vector3(u0, 0, v0) + sphere.Center,
                         new Vector3(u1, 0, v1) + sphere.Center);

                // yz
                DrawLine(new Vector3(0, u0, v0) + sphere.Center,
                         new Vector3(0, u1, v1) + sphere.Center);
            }
        }
        #endregion


        #region void Predraw(int pass)
        private void Predraw(int pass)
        {
            IGraphicsDeviceService graphicsService =
                (IGraphicsDeviceService)base.Game.Services.GetService(
                                            typeof(IGraphicsDeviceService));

            GraphicsDevice device = graphicsService.GraphicsDevice;

            currentPass = pass;

            effect.Begin();

            m_paramVP.SetValue(viewProjMatrix);
            m_paramWorld.SetValue(worldMatrix);

            effect.Techniques[0].Passes[currentPass].Begin();

            device.VertexDeclaration = vertexDecl;
        }
        #endregion

        #region void PostDraw()
        private void Postdraw()
        {
            effect.Techniques[0].Passes[currentPass].End();
            effect.End();

            currentPass = -1;
        }

        #endregion

    }
}

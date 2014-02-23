using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Game.Foundation;

namespace LD10.Game
{
    public class Camera : DrawableBehavior
    {
        [BehaviorDependency]
        protected Transform transform;

        Color clearColor = Color.CornflowerBlue;

        float near = 0.1f;
        float far = 1000.0f;

        float fov = MathHelper.PiOver4;
        float ar;

        public Camera()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            // somethings amiss!
            ar = (float)device.Viewport.Width / device.Viewport.Height;
        }

        public override void Draw()
        {
            GameContainer.Graphics.GraphicsDevice.Clear(clearColor);
        }

        public Color ClearColor
        {
            get
            {
                return clearColor;
            }
            set
            {
                clearColor = value;
            }
        }

        public float Near
        {
            get
            {
                return near;
            }
            set
            {
                near = value;
            }
        }

        public float Far
        {
            get
            {
                return far;
            }
            set
            {
                far = value;
            }
        }

        public float FieldOfView
        {
            get
            {
                return fov;
            }
            set
            {
                fov = value;
            }
        }

        public float AspectRatio
        {
            get
            {
                return ar;
            }
        }

        public virtual Matrix Projection
        {
            get
            {
                return Matrix.CreatePerspectiveFieldOfView(
                    fov,
                    ar,
                    near,
                    far);
            }
        }

        public virtual Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(
                    transform.Position,
                    Vector3.Zero,
                    Vector3.Up);
            }
        }
    }
}

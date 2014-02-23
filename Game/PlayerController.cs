using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Game.Foundation;

using LD10.Game.Springs;

namespace LD10.Game
{
    public enum ControlScheme
    {
        Keyboard = 0,
        Mouse = 1,
        Gamepad = 2
    }

    public class PlayerController : Behavior
    {
        [BehaviorDependency(Group = "Playing Field")]
        PlayingField field = null;

        ControlScheme controlScheme = ControlScheme.Gamepad;

        public ControlScheme SelectedControlScheme
        {
            get
            {
                return controlScheme;
            }
            set
            {
                controlScheme = value;
            }
        }

        PlayerIndex playerIndex;

        //bool useGamepad = true;

        SpringNode head, tail;

        float moveSpeed = 4;

        public PlayerController()
            : this(PlayerIndex.One, ControlScheme.Gamepad) { }

        public PlayerController(PlayerIndex playerIndex, ControlScheme controlScheme)
        {
            this.playerIndex = playerIndex;
            this.controlScheme = controlScheme;
        }

        public override void Update(TimeSpan elapsed)
        {
            switch (controlScheme) {
                case ControlScheme.Gamepad: {
                        GamePadState gp = GamePad.GetState(playerIndex);

                        float lX = gp.ThumbSticks.Left.X;
                        float lY = gp.ThumbSticks.Left.Y;

                        head.Velocity.X += (moveSpeed * lX) * (float)elapsed.TotalSeconds;
                        head.Velocity.Z -= (moveSpeed * lY) * (float)elapsed.TotalSeconds;
                    } break;
                case ControlScheme.Keyboard: {
                        KeyboardState ks = Keyboard.GetState();

                        if (playerIndex == PlayerIndex.One) {
                            if (ks.IsKeyDown(Keys.W)) {
                                head.Velocity.Z -= moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.S)) {
                                head.Velocity.Z += moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.A)) {
                                head.Velocity.X -= moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.D)) {
                                head.Velocity.X += moveSpeed * (float)elapsed.TotalSeconds;
                            }
                        } else if (playerIndex == PlayerIndex.Two) {
                            if (ks.IsKeyDown(Keys.Up)) {
                                head.Velocity.Z -= moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.Down)) {
                                head.Velocity.Z += moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.Left)) {
                                head.Velocity.X -= moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.Right)) {
                                head.Velocity.X += moveSpeed * (float)elapsed.TotalSeconds;
                            }
                        }
                    } break;
                case ControlScheme.Mouse: {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                            LookAtCamera camera = GameContainer.Scene.Coordinator.Select("Camera")[0].Get<LookAtCamera>();

                            Vector3 nearsource = new Vector3((float)Mouse.GetState().X, (float)Mouse.GetState().Y, 0f);
                            Vector3 farsource = new Vector3((float)Mouse.GetState().X, (float)Mouse.GetState().Y, 1f);

                            Matrix world = Matrix.CreateTranslation(0, 0, 0);

                            Vector3 nearPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(nearsource, camera.Projection, camera.View, world);
                            Vector3 farPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(farsource, camera.Projection, camera.View, world);

                            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

                            Ray ray = new Ray(nearPoint, direction);

                            // make the box a good deal larger than the size of the actual field.. 
                            // as it sucks not being able to maneuver cause you point the mouse off-field
                            BoundingBox fieldBounds = new BoundingBox(
                                new Vector3(-(field.Width * 2), -(field.Height * 2), -(field.Depth * 2)),
                                new Vector3(field.Width * 2, field.Height * 2, field.Depth * 2));

                            float? dist = ray.Intersects(fieldBounds);

                            if (dist.HasValue) {
                                Vector3 hit = ray.Position + (direction * dist.Value);

                                head.Velocity += (Vector3.Normalize(hit - head.Position) * moveSpeed) * (float)elapsed.TotalSeconds;
                                head.Velocity.Y = 0;
                            }
                        }
                    } break;
            }

            /*
            if (useGamepad) {
                GamePadState gp = GamePad.GetState(playerIndex);

                float lX = gp.ThumbSticks.Left.X;
                float lY = gp.ThumbSticks.Left.Y;

                head.Velocity.X += (moveSpeed * lX) * (float)elapsed.TotalSeconds;
                head.Velocity.Z -= (moveSpeed * lY) * (float)elapsed.TotalSeconds;

            } else {
                KeyboardState ks = Keyboard.GetState();

                switch (playerIndex) {
                    case PlayerIndex.One: {
                            if (ks.IsKeyDown(Keys.W)) {
                                head.Velocity.Z -= moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.S)) {
                                head.Velocity.Z += moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.A)) {
                                head.Velocity.X -= moveSpeed * (float)elapsed.TotalSeconds;
                            }
                            if (ks.IsKeyDown(Keys.D)) {
                                head.Velocity.X += moveSpeed * (float)elapsed.TotalSeconds;
                            }
                        } break;
                    case PlayerIndex.Two: {
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                                LookAtCamera camera = GameContainer.Scene.Coordinator.Select("Camera")[0].Get<LookAtCamera>();

                                Vector3 nearsource = new Vector3((float)Mouse.GetState().X, (float)Mouse.GetState().Y, 0f);
                                Vector3 farsource = new Vector3((float)Mouse.GetState().X, (float)Mouse.GetState().Y, 1f);

                                Matrix world = Matrix.CreateTranslation(0, 0, 0);

                                Vector3 nearPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(nearsource, camera.Projection, camera.View, world);
                                Vector3 farPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(farsource, camera.Projection, camera.View, world);

                                Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

                                Ray ray = new Ray(nearPoint, direction);

                                // make the box a good deal larger than the size of the actual field.. 
                                // as it sucks not being able to maneuver cause you point the mouse off-field
                                BoundingBox fieldBounds = new BoundingBox(
                                    new Vector3(-(field.Width * 2), -(field.Height * 2), -(field.Depth * 2)),
                                    new Vector3(field.Width * 2, field.Height * 2, field.Depth * 2));

                                float? dist = ray.Intersects(fieldBounds);

                                if (dist.HasValue) {
                                    Vector3 hit = ray.Position + (direction * dist.Value);
                                    
                                    head.Velocity += (Vector3.Normalize(hit - head.Position) * moveSpeed) * (float)elapsed.TotalSeconds;
                                    head.Velocity.Y = 0;
                                }
                            }
                        } break;
                }
            }
            */
            float w = field.Width / 2;
            float d = field.Depth / 2;

            if (head.Position.X > w) {
                head.Position.X = w;
                head.Velocity = Vector3.Zero;
            }
            if (head.Position.X < -w) {
                head.Position.X = -w;
                head.Velocity = Vector3.Zero;
            }

            if (head.Position.Z > d) {
                head.Position.Z = d;
                head.Velocity = Vector3.Zero;
            }
            if (head.Position.Z < -d) {
                head.Position.Z = -d;
                head.Velocity = Vector3.Zero;
            }

            if (tail.Position.X > w) {
                tail.Position.X = w;
                tail.Velocity = Vector3.Reflect(tail.Velocity, Vector3.Left);
            }
            if (tail.Position.X < -w) {
                tail.Position.X = -w;
                tail.Velocity = Vector3.Reflect(tail.Velocity, Vector3.Right);
            }

            if (tail.Position.Z > d) {
                tail.Position.Z = d;
                tail.Velocity = Vector3.Reflect(tail.Velocity, Vector3.Forward);
            }
            if (tail.Position.Z < -d) {
                tail.Position.Z = -d;
                tail.Velocity = Vector3.Reflect(tail.Velocity, Vector3.Backward);
            }
        }
        /*
        public bool UseGamepad
        {
            get
            {
                return useGamepad;
            }
            set
            {
                useGamepad = value;
            }
        }
        */
        public PlayerIndex PlayerIndex
        {
            get
            {
                return playerIndex;
            }
        }

        public SpringNode Head
        {
            get
            {
                return head;
            }
            set
            {
                if (head != value) {
                    head = value;
                }
            }
        }

        public SpringNode Tail
        {
            get
            {
                return tail;
            }
            set
            {
                if (tail != value) {
                    tail = value;
                }
            }
        }
    }
}

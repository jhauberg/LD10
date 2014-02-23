#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

using Game.Foundation;

using LD10.Game;
using LD10.Game.Springs;

namespace LD10
{
    public class GameContainer : Microsoft.Xna.Framework.Game
    {
        private void ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK) {
                System.Diagnostics.Debug.WriteLine("FMOD error! " + result + " - " + FMOD.Error.String(result));
            }
        }

        public static GraphicsDeviceManager Graphics;
        public static ContentManager Content;

        public static Scene Scene = new Scene();

        public static VectorRenderComponent VectorRenderer;

        public static FMOD.System SoundSystem;
        public static FMOD.Channel SoundChannel;

        SpriteBatch sprite;
        SpriteFont font, fontBold;

        Texture2D gamepad, keyboard, mouse;

        OwnGoalData ownGoal = null;

        float timeToDisplayMoron = 2;
        DateTime from = DateTime.Now;

        Random r = new Random();

        KeyboardState ksLast;

        class OwnGoalData
        {
            public PlayerIndex PlayerIndex;
            public string Message;
            public Vector2 MessageSize;

            public OwnGoalData(PlayerIndex playerIndex, string message, Vector2 messageSize)
            {
                this.PlayerIndex = playerIndex;
                this.Message = message;
                this.MessageSize = messageSize;
            }
        }

        public GameContainer()
        {
            this.Window.Title = "LD10 - Dick Chainey";
            this.IsMouseVisible = true;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferMultiSampling = true;
            Graphics.PreferredBackBufferWidth = 800;
            Graphics.PreferredBackBufferHeight = 600;

            Content = new ContentManager(Services);
        }

        protected override void Initialize()
        {
            uint version = 0;
            FMOD.RESULT result;

            result = FMOD.Factory.System_Create(ref SoundSystem);
            ERRCHECK(result);

            result = SoundSystem.getVersion(ref version);
            ERRCHECK(result);

            if (version < FMOD.VERSION.number) {
                System.Diagnostics.Debug.WriteLine("Error!  You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
            }

            result = SoundSystem.init(32, FMOD.INITFLAG.NORMAL, (IntPtr)null);
            ERRCHECK(result);

            VectorRenderer = new VectorRenderComponent(this,
                               Content,
                               "Content\\Effects\\VectorLineEffect");
            this.Components.Add(VectorRenderer);

            sprite = new SpriteBatch(Graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("Content\\Tahoma");
            fontBold = Content.Load<SpriteFont>("Content\\TahomaBold");

            gamepad = Content.Load<Texture2D>("Content\\Textures\\Controls_Gamepad");
            keyboard = Content.Load<Texture2D>("Content\\Textures\\Controls_Keyboard");
            mouse = Content.Load<Texture2D>("Content\\Textures\\Controls_Mouse");

            ////////////////////
            // register groups
            BehaviorGroup camera = Scene.Coordinator.Register("Camera");
            camera.Associate(new LookAtCamera());//new Camera());
            camera.Associate(new CameraController());

            BehaviorGroup field = Scene.Coordinator.Register("Playing Field");
            field.Associate(new PlayingField(20, 0.25f, 10));
            field.Associate(new PlayingFieldVisual());

            BehaviorGroup player1 = Scene.Coordinator.Register("Player");
            player1.Associate(new SpringSkeleton());
            player1.Associate(new PlayerController(PlayerIndex.One, ControlScheme.Gamepad));
            player1.Associate(new PlayerVisual());
            player1.Associate(new PlayerTailRepresentation());

            BehaviorGroup player2 = Scene.Coordinator.Register("Player");
            player2.Associate(new SpringSkeleton());
            player2.Associate(new PlayerController(PlayerIndex.Two, ControlScheme.Mouse));
            player2.Associate(new PlayerVisual());
            player2.Associate(new PlayerTailRepresentation());

            BehaviorGroup ball = Scene.Coordinator.Register("Ball");
            ball.Associate(new BallVisual());
            ball.Associate(new BallController());
            ball.Associate(new BallRepresentation());

            BehaviorGroup player1Score = Scene.Coordinator.Register("Score Counter");
            player1Score.Associate(new ScoreCount());
            player1Score.Associate(new ScoreVisual());

            BehaviorGroup goal1 = Scene.Coordinator.Register("Goal");
            goal1.Associate(new GoalRepresentation(PlayerIndex.One));
            goal1.Associate(new GoalVisual());

            BehaviorGroup goal2 = Scene.Coordinator.Register("Goal");
            goal2.Associate(new GoalRepresentation(PlayerIndex.Two));
            goal2.Associate(new GoalVisual());

            BehaviorGroup advertisement1 = Scene.Coordinator.Register("Commercial");
            advertisement1.Associate(new BarrierVisual());
            BehaviorGroup advertisement2 = Scene.Coordinator.Register("Commercial");
            advertisement2.Associate(new BarrierVisual());

            BehaviorGroup collisionChecks = Scene.Coordinator.Register("Collision Check");
            collisionChecks.Associate(new CollisionCheck());
            
            ////////////////////
            // set defaults
            camera.Get<Transform>().Position = new Vector3(0, 12.5f, 14);

            collisionChecks.Get<CollisionCheck>().OwnGoal += new EventHandler<OwnGoalEventArgs>(Collision_OwnGoal);

            advertisement1.Get<Transform>().Position = new Vector3(-(field.Get<PlayingField>().Width / 4), 0.75f, -(field.Get<PlayingField>().Depth / 2));
            advertisement1.Get<Transform>().Scale = new Vector3((field.Get<PlayingField>().Width / 2) - 0.5f, 1, 1);
            advertisement1.Get<Transform>().Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(-35));
            advertisement1.Get<BarrierVisual>().Advertisement = Content.Load<Texture2D>("Content\\Textures\\Groov");

            advertisement2.Get<Transform>().Position = new Vector3((field.Get<PlayingField>().Width / 4), 0.75f, -(field.Get<PlayingField>().Depth / 2));
            advertisement2.Get<Transform>().Scale = new Vector3((field.Get<PlayingField>().Width / 2) - 0.5f, 1, 1);
            advertisement2.Get<Transform>().Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(-35));
            advertisement2.Get<BarrierVisual>().Advertisement = Content.Load<Texture2D>("Content\\Textures\\Midget");

            goal1.Get<Transform>().Position = new Vector3(-(field.Get<PlayingField>().Width / 2) - (goal1.Get<GoalRepresentation>().Extents.X / 2) + 0.5f, goal1.Get<GoalRepresentation>().Extents.Y / 2, 0);
            goal2.Get<Transform>().Position = new Vector3((field.Get<PlayingField>().Width / 2) + (goal2.Get<GoalRepresentation>().Extents.X / 2) - 0.5f, goal2.Get<GoalRepresentation>().Extents.Y / 2, 0);

            // player1
            SpringSkeleton skeleton = player1.Get<SpringSkeleton>();

            skeleton.Coefficient = 0.3f;
            skeleton.Elasticity = 0.01f;
            skeleton.EnergyLoss = 0.86f;

            skeleton.Enabled = true;

            SpringHelper.CreateChain(skeleton, 5, 0.35f, new Vector3(-1, 0, 0), false);

            skeleton.Nodes[skeleton.Nodes.Count - 1].Mass = 2;

            player1.Get<PlayerController>().Head = skeleton.Nodes[0];
            player1.Get<PlayerController>().Tail = skeleton.Nodes[skeleton.Nodes.Count - 1];

            // player2
            SpringSkeleton skeleton2 = player2.Get<SpringSkeleton>();

            skeleton2.Coefficient = 0.3f;
            skeleton2.Elasticity = 0.01f;
            skeleton2.EnergyLoss = 0.86f;

            skeleton2.Enabled = true;

            SpringHelper.CreateChain(skeleton2, 5, 0.35f, new Vector3(1, 0, 0), true);

            skeleton2.Nodes[skeleton2.Nodes.Count - 1].Mass = 2;

            player2.Get<PlayerController>().Head = skeleton2.Nodes[0];
            player2.Get<PlayerController>().Tail = skeleton2.Nodes[skeleton2.Nodes.Count - 1];

            base.Initialize();

            Scene.Start();

            // hack - this is because the dependency to the PlayingField behavior has not been injected before Scene.Start()
            ball.Get<BallController>().Reset();
        }

        void Collision_OwnGoal(object sender, OwnGoalEventArgs e)
        {
            // where 3 is the amount of fail messages currently.. yea yea, hardcoded..
            int n = r.Next(0, 5);

            string msg = "";

            switch (n) {
                case 0: msg = Fail1; break;
                case 1: msg = Fail2; break;
                case 2: msg = Fail3; break;
                case 3: msg = Fail4; break;
                case 4: msg = Fail5; break;
            }

            ownGoal = new OwnGoalData(e.PlayerIndex, msg, fontBold.MeasureString(msg));

            from = DateTime.Now;
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent) {
                Content.Unload();
            }
            
            FMOD.RESULT result;

            if (SoundSystem != null) {
                result = SoundSystem.close();
                ERRCHECK(result);
                result = SoundSystem.release();
                ERRCHECK(result);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Scene.Update(gameTime.ElapsedGameTime);

            if (ownGoal != null) {
                TimeSpan since = DateTime.Now - from;

                if (since.TotalSeconds > timeToDisplayMoron) {
                    ownGoal = null;
                }
            }

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyUp(Keys.Z) && ksLast.IsKeyDown(Keys.Z)) {
                // cycle player 1
                PlayerController controller = Scene.Coordinator.Select("Player")[0].Get<PlayerController>();

                controller.SelectedControlScheme += 1;

                string[] values = Enum.GetNames(typeof(ControlScheme));
                if (controller.SelectedControlScheme > (ControlScheme)Enum.Parse(typeof(ControlScheme), values[values.Length - 1])) {
                    controller.SelectedControlScheme = 0;
                }
            } if (ks.IsKeyUp(Keys.X) && ksLast.IsKeyDown(Keys.X)) {
                // cycle player 2
                PlayerController controller = Scene.Coordinator.Select("Player")[1].Get<PlayerController>();

                controller.SelectedControlScheme += 1;

                string[] values = Enum.GetNames(typeof(ControlScheme));
                if (controller.SelectedControlScheme > (ControlScheme)Enum.Parse(typeof(ControlScheme), values[values.Length - 1])) {
                    controller.SelectedControlScheme = 0;
                }
            }

            ksLast = ks;

            base.Update(gameTime);
        }

        const string Fail1 = "M-m-m-monster fail!";
        const string Fail2 = "MORON LOL!";
        const string Fail3 = "This failure message is sponsored by midgetwrestling.com";
        const string Fail4 = "klaphat";
        const string Fail5 = "for helvede det er sent";

        const string CycleInfo = "Press 'z' and 'x' to cycle control-schemes";

        const string Player1 = "Player 1";
        const string Player2 = "Player 2";

        protected override void Draw(GameTime gameTime)
        {
            Scene.Draw();

            sprite.Begin(SpriteBlendMode.AlphaBlend);

            sprite.DrawString(font, Player1, new Vector2(21, 11), Color.Black);
            sprite.DrawString(font, Player1, new Vector2(20, 10), Color.White);

            switch (Scene.Coordinator.Select("Player")[0].Get<PlayerController>().SelectedControlScheme) {
                case ControlScheme.Gamepad: {
                        sprite.Draw(gamepad, new Vector2(30, 35), Color.White);
                    } break;
                case ControlScheme.Keyboard: {
                        sprite.Draw(keyboard, new Vector2(30, 35), Color.White);
                    } break;
                case ControlScheme.Mouse: {
                        sprite.Draw(mouse, new Vector2(30, 35), Color.White);
                    } break;
            }

            sprite.DrawString(font, Player2, new Vector2(Graphics.GraphicsDevice.Viewport.Width - 89, 11), Color.Black);
            sprite.DrawString(font, Player2, new Vector2(Graphics.GraphicsDevice.Viewport.Width - 90, 10), Color.White);

            switch (Scene.Coordinator.Select("Player")[1].Get<PlayerController>().SelectedControlScheme) {
                case ControlScheme.Gamepad: {
                        sprite.Draw(gamepad, new Vector2(Graphics.GraphicsDevice.Viewport.Width - 110, 35), Color.White);
                    } break;
                case ControlScheme.Keyboard: {
                        sprite.Draw(keyboard, new Vector2(Graphics.GraphicsDevice.Viewport.Width - 110, 35), Color.White);
                    } break;
                case ControlScheme.Mouse: {
                        sprite.Draw(mouse, new Vector2(Graphics.GraphicsDevice.Viewport.Width - 110, 35), Color.White);
                    } break;
            }

            float cycleLength = font.MeasureString(CycleInfo).X;

            sprite.DrawString(font, CycleInfo, new Vector2((Graphics.GraphicsDevice.Viewport.Width / 2) - (cycleLength / 2) + 1, 11), Color.Black);
            sprite.DrawString(font, CycleInfo, new Vector2((Graphics.GraphicsDevice.Viewport.Width / 2) - (cycleLength / 2), 10), Color.White);
           
            ScoreCount score = Scene.Coordinator.Select("Score Counter")[0].Get<ScoreCount>();

            sprite.DrawString(fontBold, String.Format("Tally: {0} - {1}", score.GetTally(PlayerIndex.One), score.GetTally(PlayerIndex.Two)), new Vector2((Graphics.GraphicsDevice.Viewport.Width / 2) - 69, 46), Color.Black);
            sprite.DrawString(fontBold, String.Format("Tally: {0} - {1}", score.GetTally(PlayerIndex.One), score.GetTally(PlayerIndex.Two)), new Vector2((Graphics.GraphicsDevice.Viewport.Width / 2) - 70, 45), Color.White);

            if (ownGoal != null) {
                sprite.DrawString(fontBold, ownGoal.Message, new Vector2((Graphics.GraphicsDevice.Viewport.Width / 2) - (ownGoal.MessageSize.X / 2) + 1, Graphics.GraphicsDevice.Viewport.Height - 70 + 1), Color.Black);
                sprite.DrawString(fontBold, ownGoal.Message, new Vector2((Graphics.GraphicsDevice.Viewport.Width / 2) - (ownGoal.MessageSize.X / 2), Graphics.GraphicsDevice.Viewport.Height - 70), Color.White);
            }

            sprite.End();

            base.Draw(gameTime);
        }
    }
}

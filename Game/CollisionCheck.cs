using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Game.Foundation;

using LD10.Game.Springs;

namespace LD10.Game
{
    public class CollisionCheck : Behavior
    {
        [BehaviorDependency(Group = "Ball")]
        BallRepresentation ball = null;

        float winWaitSeconds = 1;

        bool scored;
        DateTime from = DateTime.Now;

        // todo: something
        FMOD.Sound hit1, hit2, hit3;
        FMOD.Sound score;

        Random r = new Random();

        PlayerIndex lastPlayerThatHit = default(PlayerIndex);

        public event EventHandler<OwnGoalEventArgs> OwnGoal;
        private void OnOwnGoal(PlayerIndex playerIndex)
        {
            if (OwnGoal != null) {
                OwnGoal(this, new OwnGoalEventArgs(playerIndex));
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            GameContainer.SoundSystem.createSound("Content\\Sounds\\hit1.wav", FMOD.MODE.HARDWARE, ref hit1);
            GameContainer.SoundSystem.createSound("Content\\Sounds\\hit2.wav", FMOD.MODE.HARDWARE, ref hit2);
            GameContainer.SoundSystem.createSound("Content\\Sounds\\hit3.wav", FMOD.MODE.HARDWARE, ref hit3);

            hit1.setMode(FMOD.MODE.LOOP_OFF);
            hit2.setMode(FMOD.MODE.LOOP_OFF);
            hit3.setMode(FMOD.MODE.LOOP_OFF);

            GameContainer.SoundSystem.createSound("Content\\Sounds\\score.wav", FMOD.MODE.HARDWARE, ref score);

            score.setMode(FMOD.MODE.LOOP_OFF);
        }

        public override void Update(TimeSpan elapsed)
        {
            foreach (BehaviorGroup group in GameContainer.Scene.Coordinator.Select("Player")) {
                BehaviorGroup field = GameContainer.Scene.Coordinator.Select("Playing Field")[0];

                float offset = 0.3f;
                float floor = field.Get<Transform>().Position.Y + (field.Get<PlayingField>().Height / 2) + offset;

                foreach (SpringNode node in group.Get<SpringSkeleton>().Nodes) {
                    node.Velocity *= 1.1f; // ice skating!

                    if (node.Position.Y <= floor) {
                        node.Position = new Vector3(node.Position.X, floor, node.Position.Z);
                    }
                }

                PlayerTailRepresentation tail = group.Get<PlayerTailRepresentation>();

                if (tail.BoundingSphere.Intersects(ball.BoundingSphere)) {
                    ball.Group.Get<BallController>().Push(Vector3.Normalize(ball.BoundingSphere.Center - tail.BoundingSphere.Center), elapsed);

                    lastPlayerThatHit = tail.Group.Get<PlayerController>().PlayerIndex;

                    GameContainer.SoundSystem.playSound(FMOD.CHANNELINDEX.FREE, (r.Next(0, 2) > 0 ? hit2 : hit3), false, ref GameContainer.SoundChannel);
                }
            }

            foreach (BehaviorGroup group in GameContainer.Scene.Coordinator.Select("Goal")) {
                GoalRepresentation goal = group.Get<GoalRepresentation>();

                if (!scored) {
                    if (ball.BoundingSphere.Intersects(goal.BoundingBox)) {
                        GameContainer.Scene.Coordinator.Select("Score Counter")[0].Get<ScoreCount>().IncreaseScore(
                            goal.PlayerIndex == PlayerIndex.One ?
                                PlayerIndex.Two :
                                PlayerIndex.One);

                        if (!scored) {
                            scored = true;
                            from = DateTime.Now;

                            GameContainer.SoundSystem.playSound(FMOD.CHANNELINDEX.FREE, score, false, ref GameContainer.SoundChannel);

                            if (lastPlayerThatHit == goal.PlayerIndex) {
                                OnOwnGoal(goal.PlayerIndex);
                            }
                        }
                    }
                }

                foreach (SpringNode node in group.Get<SpringSkeleton>().Nodes) {
                    ContainmentType ct = default(ContainmentType);

                    ct = ball.BoundingSphere.Contains(node.Position);

                    if (ct == ContainmentType.Contains || ct == ContainmentType.Intersects) {
                        ball.Group.Get<BallController>().velocity *= 0.75f;
                        node.Velocity += (Vector3.Normalize(node.Position - ball.BoundingSphere.Center) * 8) * (float)elapsed.TotalSeconds;
                        //node.Position = ClosestPointOnSphericalSurface(ball.BoundingSphere.Center, ball.BoundingSphere.Radius, node.Position);
                    }
                    /* i dont get why this doesnt work
                    foreach (BehaviorGroup player in GameContainer.Scene.Coordinator.Select("Player")) {
                        PlayerTailRepresentation tail = player.Get<PlayerTailRepresentation>();
                        
                        ct = tail.BoundingSphere.Contains(node.Position);

                        if (ct == ContainmentType.Contains || ct == ContainmentType.Intersects) {
                            node.Velocity += (Vector3.Normalize(node.Position - tail.BoundingSphere.Center) * 8) * (float)elapsed.TotalSeconds;
                        }
                    }*/
                }
            }

            if (scored) {
                TimeSpan since = DateTime.Now - from;

                if (since.TotalSeconds > winWaitSeconds) {
                    scored = false;
                    ball.Group.Get<BallController>().Reset();
                }
            }
        }
        /*
        public static Vector3 ClosestPointOnSphericalSurface(Vector3 center, float radius, Vector3 p)
        {
            double a = radius / (
                Math.Sqrt(
                    Math.Pow((double)(p.X - center.X), 2.0) +
                    Math.Pow((double)(p.Y - center.Y), 2.0) +
                    Math.Pow((double)(p.Z - center.Z), 2.0)));

            return
                new Vector3(
                    p.X * (float)a,
                    p.Y * (float)a,
                    p.Z * (float)a);
        }*/
    }
}

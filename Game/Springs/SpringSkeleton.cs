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
using System.Collections.ObjectModel;

namespace LD10.Game.Springs
{
    public class SpringSkeleton : Behavior
    {
        float energyLoss = 0.98f;
        float k = 0.3f; // coefficient

        float elasticity = 0.1f; // higher = more elastic springs

        Vector3 gravity = new Vector3(0, -9.81f, 0);

        List<SpringNode> nodes = new List<SpringNode>();
        List<SpringNodeAnchor> anchors = new List<SpringNodeAnchor>();

        public SpringSkeleton() { }

        public SpringSkeleton(float k, float energyLoss, Vector3 gravity)
        {
            this.energyLoss = energyLoss;
            this.gravity = gravity;
            this.k = k;
        }
        public override void Initialize()
        {
            base.Initialize();

            Enabled = false; // start simulation as paused
        }

        public override void Update(TimeSpan elapsed)
        {
            AccumulateForces();
            ApplyForces(elapsed);
            AnchorCheck();
        }

        private void AccumulateForces()
        {
            Vector3 source, dest, resultant;

            float distance;

            foreach (SpringNode node in nodes) {
                resultant = Vector3.Zero;
                source = node.Position;

                foreach (SpringNode neighborNode in node.Neighbors.Keys) {
                    dest = neighborNode.Position;
                    distance = node.Neighbors[neighborNode];

                    Vector3 force = SpringNode.ComputeSingleForce(
                        source,
                        dest,
                        k,
                        distance);

                    resultant += force;
                }

                node.Force = resultant;
            }
        }

        private void ApplyForces(TimeSpan elapsed)
        {
            Vector3 a = Vector3.Zero;
            Vector3 g = gravity * elasticity;

            foreach (SpringNode node in nodes) {
                a = (node.Force + (g * (float)elapsed.TotalSeconds)) / node.Mass;

                node.Velocity += a;
                node.Position += node.Velocity;
                node.Velocity *= energyLoss;
            }
        }

        private void AnchorCheck()
        {
            foreach (SpringNodeAnchor anchor in anchors) {
                if (anchor.Node.Position != anchor.Position) {
                    anchor.Node.Position = anchor.Position;
                }
            }
        }
        /*
        public ReadOnlyCollection<SpringNode> Nodes
        {
            get
            {
                return new ReadOnlyCollection<SpringNode>(nodes);
            }
        }

        public ReadOnlyCollection<SpringNodeAnchor> Anchors
        {
            get
            {
                return new ReadOnlyCollection<SpringNodeAnchor>(anchors);
            }
        }*/

        public List<SpringNode> Nodes
        {
            get
            {
                return nodes;
            }
        }

        public List<SpringNodeAnchor> Anchors
        {
            get
            {
                return anchors;
            }
        }

        public float EnergyLoss
        {
            get
            {
                return energyLoss;
            }
            set
            {
                energyLoss = value;
            }
        }

        public float Elasticity
        {
            get
            {
                return elasticity;
            }
            set
            {
                elasticity = value;
            }
        }

        public float Coefficient
        {
            get
            {
                return k;
            }
            set
            {
                k = value;
            }
        }

        public Vector3 Gravity
        {
            get
            {
                return gravity;
            }
            set
            {
                gravity = value;
            }
        }
    }
}

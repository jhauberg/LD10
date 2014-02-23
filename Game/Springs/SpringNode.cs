
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

namespace LD10.Game.Springs
{
    public class SpringNode
    {
        public Vector3 Position;
        public Vector3 Velocity;
        Vector3 force;

        float mass = 1.0f;

        Dictionary<SpringNode, float> neighbors = new Dictionary<SpringNode, float>(); // neighbor, distance

        // empty constructor to allow use of PropertyGrid to add nodes
        public SpringNode()
            : this(Vector3.Zero) { }

        public SpringNode(Vector3 position)
            : this(position, 1) { }

        public SpringNode(Vector3 position, float mass)
        {
            this.Position = position;
            this.mass = mass;
        }

        public Dictionary<SpringNode, float> Neighbors
        {
            get
            {
                return neighbors;
            }
        }

        public float Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
            }
        }
        /*
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }*/
        /*
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }*/

        public Vector3 Force
        {
            get
            {
                return force;
            }
            set
            {
                force = value;
            }
        }

        public static bool IsAnchor(SpringNode node, SpringSkeleton skeleton)
        {
            // necessary to store the reference somewhere
            SpringNodeAnchor anchor;

            return SpringNode.IsAnchor(node, skeleton, out anchor);
        }

        public static bool IsAnchor(SpringNode node, SpringSkeleton skeleton, out SpringNodeAnchor anchor)
        {
            anchor = null;
            for (int i = 0; i < skeleton.Anchors.Count; i++) {
                if (skeleton.Anchors[i].Node == node) {
                    anchor = skeleton.Anchors[i];

                    return true;
                }
            }

            return false;
        }

        public static Vector3 ComputeSingleForce(Vector3 pa, Vector3 pb, float k, float distance)
        {
            float springTolerance = 0.005f;//0.0000000005f;

            Vector3 forceDirection = Vector3.Zero;

            float intensity;
            float d;
            float delta;

            d = Vector3.Distance(pa, pb);

            if (d < springTolerance) {
                return forceDirection;
            }

            forceDirection = Vector3.Normalize(pb - pa);

            delta = d - distance;
            intensity = k * delta;

            return forceDirection * intensity;
        }
    }
}

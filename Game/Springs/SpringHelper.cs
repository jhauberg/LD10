using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LD10.Game.Springs
{
    public static class SpringHelper
    {
        public static void CreateChain(SpringSkeleton skeleton, int parts, float partDistance, Vector3 origin, bool right)
        {
            SpringNode start = new SpringNode(origin);

            skeleton.Nodes.Add(start);

            SpringNode previous = start;

            for (int i = 0; i < parts; i++) {
                // hack
                Vector3 newPosition = Vector3.Zero;

                if (right) {
                    newPosition =
                        previous.Position +
                        new Vector3(partDistance, 0, 0);
                } else {
                    newPosition =
                        previous.Position -
                        new Vector3(partDistance, 0, 0);
                }

                SpringNode next = new SpringNode(newPosition);

                previous.Neighbors[next] = partDistance;
                next.Neighbors[previous] = partDistance;

                skeleton.Nodes.Add(next);

                previous = next;
            }

            SpringNode end = new SpringNode(previous.Position + new Vector3(partDistance, 0, 0));

            previous.Neighbors[end] = partDistance;
            end.Neighbors[previous] = partDistance;

            skeleton.Nodes.Add(end);
        }
    }
}


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
    public class SpringNodeAnchor
    {
        SpringNode node;
        Vector3 position;

        public SpringNodeAnchor(SpringNode node)
        {
            this.node = node;
            this.position = node.Position;
        }

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
        }

        public SpringNode Node
        {
            get
            {
                return node;
            }
            set
            {
                if (node != value) {
                    node = value;
                    position = node.Position;
                }
            }
        }
    }
}

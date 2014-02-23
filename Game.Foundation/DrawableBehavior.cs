using System;
using System.Collections.Generic;

namespace Game.Foundation
{
    /// <summary>
    /// Represents a modular behavior that has visual properties.
    /// </summary>
    public abstract class DrawableBehavior : Behavior
    {
        bool visible = true;

        /// <summary>
        /// Handles behavior specific drawing.
        /// </summary>
        public virtual void Draw() { }

        /// <summary>
        /// Gets or sets whether this behavior is to be drawn.
        /// </summary>
        public virtual bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (visible != value) {
                    visible = value;

                    // fire changed
                }
            }
        }
    }
}

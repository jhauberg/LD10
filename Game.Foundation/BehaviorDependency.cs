using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Foundation
{
    /// <summary>
    /// Indicates that the field is a dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class BehaviorDependency : Attribute
    {
        string group;

        int index;

        /// <summary>
        /// Gets or sets the name of the group the dependency reference should be retrieved from.
        /// </summary>
        /// <remarks>
        /// The group can be the local group, but if specified, then injection is delayed until Start. (i.e. not during Initialize)
        /// </remarks>
        public string Group
        {
            get
            {
                return group;
            }
            set
            {
                group = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the group in the coordinators list.
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Foundation
{
    /// <summary>
    /// Represents a modular behavior.
    /// </summary>
    public abstract class Behavior
    {
        BehaviorGroup group;
        
        bool enabled = true;

        /// <summary>
        /// Initializes behavior specific variables, and injects local dependencies.
        /// </summary>
        public virtual void Initialize()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields) {
                object[] attributes = field.GetCustomAttributes(typeof(BehaviorDependency), false);

                foreach (BehaviorDependency attribute in attributes) {
                    string groupName = attribute.Group;

                    if (groupName != null && groupName.Length > 0) {
                        break; // get outta here, as explicitly specified group names is another way of saying "dont associate this until Start"
                    }

                    if (!field.FieldType.IsSubclassOf(typeof(Behavior))) {
                        throw new ArgumentException("This field can not be marked as a dependency.", field.Name); // get outta here, as the field is not a behavior
                    }

                    if (group.IsAssociated(field.FieldType)) {
                        // behavior already associated, so inject reference
                        field.SetValue(this, group.Get(field.FieldType));

                        break;
                    }

                    // at this point the type should have been verified; meaning, it should be a Behavior
                    // and not anything else, ie. not any arbitrary class that does not inherit from Behavior.
                    Behavior behavior = null;

                    try {
                        behavior = Activator.CreateInstance(field.FieldType) as Behavior;
                    } catch (MissingMethodException) {
                        throw new MissingMethodException("The behavior dependency does not implement an empty constructor.");
                    }

                    group.Associate(behavior);

                    // inject newly created reference
                    field.SetValue(this, behavior);
                }
            }
        }

        /// <summary>
        /// Initializes behavior specific variables that are affected by data from outside the group, and injects non-local dependencies.
        /// </summary>
        public virtual void Start() 
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            foreach (FieldInfo field in fields) {
                object[] attributes = field.GetCustomAttributes(typeof(BehaviorDependency), false);

                foreach (BehaviorDependency attribute in attributes) {
                    string groupName = attribute.Group;

                    if (groupName == null || !(groupName.Length > 0)) {
                        break;
                    }

                    if (!field.FieldType.IsSubclassOf(typeof(Behavior))) {
                        // get outta here, as the field is not a behavior
                        throw new ArgumentException("This field can not be marked as a dependency.", field.Name); 
                    }

                    BehaviorGroup externalGroup = group.Coordinator.Select(groupName)[attribute.Index];

                    if (externalGroup.IsAssociated(field.FieldType)) {
                        field.SetValue(this, externalGroup.Get(field.FieldType));

                        break;
                    }

                    Behavior behavior = null;

                    try {
                        behavior = Activator.CreateInstance(field.FieldType) as Behavior;
                    } catch (MissingMethodException) {
                        throw new MissingMethodException("The behavior dependency does not implement an empty constructor.");
                    }

                    externalGroup.Associate(behavior);

                    // inject newly created reference
                    field.SetValue(this, behavior);
                }
            }
        }

        /// <summary>
        /// Handles behavior specific logic.
        /// </summary>
        /// <param name="elapsed">The amount of time spent since previous frame.</param>
        public virtual void Update(TimeSpan elapsed) { }

        /// <summary>
        /// Gets the group this behavior is associated with.
        /// </summary>
        public BehaviorGroup Group
        {
            get
            {
                return group;
            }
            internal set
            {
                if (group != value) {
                    group = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this behavior is to be updated.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value) {
                    enabled = value;
                }
            }
        }
    }
}

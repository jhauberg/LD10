using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Foundation
{
    // reasoning for not just extending ICollection is due to method naming ideas, not sure if it's a bad idea not extending..
    // basically supposed to provide the same functionality really
    /// <summary>
    /// Represents a group of unique behaviors.
    /// </summary>
    public class BehaviorGroup : IEnumerable<Behavior>
    {
        Dictionary<Type, Behavior> 
            behaviors = new Dictionary<Type, Behavior>();

        // events, OnAssociated, OnDissociated, OnDisband, OnParentChanged

        string name;

        BehaviorGroupCoordinator coordinator;

        // make internal constructor, one used by coordinator that does not take a coordinator parameter.. this way
        // we can then instantiate groups and specify coordinator there - or, just ask coordinator to register under a name..
        internal BehaviorGroup(string name) 
        {
            this.name = name;
        }

        // TODO: no reason to make .Name setter internal, just pass it through constructor (all three)..

        public BehaviorGroup(string name, BehaviorGroupCoordinator coordinator)
            : this(name, coordinator, new Behavior[] { }) { }

        public BehaviorGroup(string name, BehaviorGroupCoordinator coordinator, params Behavior[] behaviors)
        {
            this.name = name;
            this.coordinator = coordinator;
            
            // register prior to associating behaviors, as they may (should not, but meh!) wish to communicate throughout the system
            // the method called is internal to prevent happenings such as registering the same group under different names
            coordinator.Register(this);

            if (behaviors.Length > 0) {
                Associate(behaviors);
            }
        }

        /// <summary>
        /// Associates a behavior with the group. This behavior must not already be associated.
        /// </summary>
        /// <param name="behavior">The behavior instance to associate.</param>
        public void Associate(Behavior behavior)
        {
            if (behavior == null) {
                throw new ArgumentException("Behavior can not be null.", "behavior");
            }
            
            // Exception is too general (fxcop rule)
            if (behaviors.ContainsValue(behavior)) {
                throw new ArgumentException("A behavior group can not be associated with multiple references to the same instance.", "behavior");
            }
            if (behaviors.ContainsKey(behavior.GetType())) {
                throw new ArgumentException("A behavior group can not contain multiple instances of the same type.", "behavior");
            }

            behavior.Group = this;

            behaviors.Add(behavior.GetType(), behavior);

            // initialize lastly, as a stupid user might specify the behavior itself as a dependency and thus not 
            // getting the correct instance registered/added -- what? this does not make any sense
            behavior.Initialize();
        }

        /// <summary>
        /// Associates a range of behaviors with the group.
        /// </summary>
        /// <param name="behaviors">The range of behaviors to associate.</param>
        public void Associate(IEnumerable<Behavior> behaviors)
        {
            foreach (Behavior behavior in behaviors) {
                Associate(behavior);
            }
        }

        // todo: re-initialize all behaviors, since a dependency behavior might have been removed!
        // .. or, first go through and figure out if the one being removed is a dependency - this is probably better
        // first solution would result in new behavior instances being added, and old one lost

        /// <summary>
        /// Dissociates the behavior matching the specified type with the group.
        /// </summary>
        /// <param name="behavior">The type of the behavior to dissociate.</param>
        public void Dissociate(Type behavior)
        {
            if (behaviors.ContainsKey(behavior)) {
                behaviors[behavior].Group = null;
                behaviors.Remove(behavior);
            }
        }

        /// <summary>
        /// Dissociates the specific behavior with the group.
        /// </summary>
        /// <param name="behavior">The behavior to dissociate.</param>
        public void Dissociate(Behavior behavior)
        {
            if (behaviors.ContainsValue(behavior)) {
                behaviors.Remove(behavior.GetType());
                behavior.Group = null;
            }
        }

        /// <summary>
        /// Dissociates all behaviors with the group.
        /// </summary>
        public void Disband()
        {
            foreach (Behavior behavior in behaviors.Values) {
                behavior.Group = null;
            }
            
            behaviors.Clear();
        }

        /// <summary>
        /// Determines whether the group is associated with a behavior of the specific type.
        /// </summary>
        /// <param name="type">The type of the behavior.</param>
        /// <returns></returns>
        public bool IsAssociated(Type type)
        {
            foreach (Type behaviorType in behaviors.Keys) {
                if (behaviorType.Equals(type)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the group is associated with a behavior of the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the behavior.</typeparam>
        /// <returns></returns>
        public bool IsAssociated<T>() where T : Behavior
        {
            return IsAssociated(typeof(T));
        }

        /// <summary>
        /// Returns a reference to the behavior of the specific type, assuming the type is associated with this group.
        /// </summary>
        /// <typeparam name="T">The type of the behavior.</typeparam>
        /// <returns></returns>
        public T Get<T>() where T : Behavior
        {
            return behaviors[typeof(T)] as T;
        }
        /*
        public T Get<T>(T t) where T : Behavior
        {
            return Get<T>();
        }*/
        /// <summary>
        /// Returns a reference to the behavior of the specific type, assuming the type is associated with this group.
        /// </summary>
        /// <param name="t">The type of the behavior</param>
        /// <returns></returns>
        public Behavior Get(Type type)
        {
            return behaviors[type];
        }

        /// <summary>
        /// Returns a reference to the first behavior that is a sub-class of, or actually is, the specific type, 
        /// assuming the type is associated with this group.
        /// </summary>
        /// <typeparam name="T">The type of the behavior.</typeparam>
        /// <returns></returns>
        public T GetOf<T>() where T : Behavior
        {
            foreach (Behavior behavior in behaviors.Values) {
                if (behavior is T) {
                    return behaviors[behavior.GetType()] as T;
                }
            }

            return default(T);
        }

        // possible feature:
        // GetAll<T>() returns all behaviors of type T
        // this would be a simple loop with type check, but might not be useful at all

        // might be a bad idea creating the list each time, might be a better
        // solution to create the list each time something is added/removed

        /// <summary>
        /// Gets a read-only list of the behaviors associated with this group.
        /// </summary>
        private ReadOnlyCollection<Behavior> Behaviors
        {
            get
            {
                Behavior[] tmp = new Behavior[behaviors.Values.Count];
                behaviors.Values.CopyTo(tmp, 0);

                return new ReadOnlyCollection<Behavior>(tmp);
            }
        }

        /// <summary>
        /// Gets the name this group is registered under.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the coordinator associated with this group.
        /// </summary>
        public BehaviorGroupCoordinator Coordinator
        {
            get
            {
                return coordinator;
            }
            // made internal so that it can be nullified upon removal from coordinator
            internal set
            {
                coordinator = value;
            }
        }

        /// <summary>
        /// Gets the amount of behaviors this group is associated with.
        /// </summary>
        public int Count
        {
            get
            {
                return Behaviors.Count;
            }
        }

        /// <summary></summary>
        /// <param name="index">The zero-based index of the behavior to get.</param>
        /// <returns></returns>
        public Behavior this[int index]
        {
            get
            {
                return Behaviors[index];
            }
        }

        #region IEnumerable<Behavior> Members

        public IEnumerator<Behavior> GetEnumerator()
        {
            return Behaviors.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            string contents = "";

            foreach (Type type in behaviors.Keys) {
                contents += type.ToString() + ", ";
            }

            return "{ " + contents.Remove(contents.LastIndexOf(',')) + " }";
        }
    }
}

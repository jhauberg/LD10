using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Foundation
{
    /// <summary>
    /// Represents a collection of groups, and provides each registered group with a connection to all others.
    /// </summary>
    public sealed class BehaviorGroupCoordinator
    {
        Dictionary<string, List<BehaviorGroup>>
            registrants = new Dictionary<string, List<BehaviorGroup>>();

        // events, OnRegistered, OnUnregistered

        /// <summary>
        /// Registers a group under the given name.
        /// </summary>
        /// <param name="name">The name this group is registered under.</param>
        /// <returns></returns>
        public BehaviorGroup Register(string name)
        {
            BehaviorGroup group = new BehaviorGroup(name);

            group.Coordinator = this;

            Register(group);

            return group;
        }

        // because of this overload, it is possible to register a group under multiple names
        // possibly an un-wanted feature
        // update: for now, keep it private

        /// <summary>
        /// Registers a specific group under the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        // experiment: made internal so that a group can register itself if instantiated with a constructor that takes a coordinator
        internal void Register(BehaviorGroup group)
        {
            // exceptions are not thrown before the group has actually been instanciated
            // this means garbage can potentially be made constantly. best solution
            // would be to check before creating the new instance

            if (registrants.ContainsKey(group.Name)) {
                registrants[group.Name].Add(
                    group);
            } else {
                registrants.Add(
                    group.Name,
                    new List<BehaviorGroup>(
                        new BehaviorGroup[] { 
                            group }));
            }
        }

        /// <summary>
        /// Unregisters all groups under this name.
        /// </summary>
        /// <param name="name">The name to unregister.</param>
        public void Unregister(string name)
        {
            List<BehaviorGroup> groups = registrants[name];

            // make sure the specific groups are removed properly. important to do this first, prior to removing
            // the list in the dictionary as Unregister(group) requires
            for (int i = 0; i < groups.Count; i++) {
                Unregister(groups[i]);
            }

            registrants.Remove(name);
        }
        
        // with this method we wish to unregiser a specific instance
        // it is very likely that this method should be removed

        /// <summary>
        /// Unregisters a specific group.
        /// </summary>
        /// <param name="group">The reference to the group instance to unregister.</param>
        public void Unregister(BehaviorGroup group)
        {
            List<BehaviorGroup> groups = registrants[group.Name];

            if (groups.Contains(group)) {
                groups.Remove(group);

                // nullify the reference to the coordinator, as it is no longer registered
                group.Coordinator = null;
            }
        }

        /// <summary>
        /// Returns a read-only list of all groups registered to the given name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns></returns>
        public ReadOnlyCollection<BehaviorGroup> Select(string name)
        {
            return new ReadOnlyCollection<BehaviorGroup>(registrants[name]);
        }

        /// <summary>
        /// Gets a read-only list that contains the names of all registered groups.
        /// </summary>
        // todo: keep list to avoid re-creation
        public ReadOnlyCollection<string> Registrants
        {
            get
            {
                string[] tmp = new string[registrants.Keys.Count];
                registrants.Keys.CopyTo(tmp, 0);

                return new ReadOnlyCollection<string>(tmp);
            }
        }
    }
}

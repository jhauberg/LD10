using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Foundation
{
    public class Scene
    {
        BehaviorGroupCoordinator coordinator = new BehaviorGroupCoordinator();

        /// <summary>
        /// Signals that it is time to update logic.
        /// </summary>
        /// <param name="elapsed">The time passed since last frame.</param>
        public void Update(TimeSpan elapsed)
        {
            foreach (string registrant in coordinator.Registrants) {
                ReadOnlyCollection<BehaviorGroup> groups = coordinator.Select(registrant);

                for (int i = 0; i < groups.Count; i++) {
                    // foreach'ing == trying to remove behavior during run-time == exception
                    for (int j = 0; j < groups[i].Count; j++) {
                        if (groups[i][j].Enabled) {
                            groups[i][j].Update(elapsed);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Signals that it is time to draw things on the screen.
        /// </summary>
        public void Draw()
        {
            foreach (string registrant in coordinator.Registrants) {
                ReadOnlyCollection<BehaviorGroup> groups = coordinator.Select(registrant);

                for (int i = 0; i < groups.Count; i++) {
                    for (int j = 0; j < groups[i].Count; j++) {
                        if (groups[i][j] is DrawableBehavior) {
                            DrawableBehavior drawable = groups[i][j] as DrawableBehavior;

                            if (drawable.Visible) {
                                drawable.Draw();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Signals that all initial groups have been registered.
        /// </summary>
        public void Start()
        {
            foreach (string registrant in coordinator.Registrants) {
                ReadOnlyCollection<BehaviorGroup> groups = coordinator.Select(registrant);

                for (int i = 0; i < groups.Count; i++) {
                    for (int j = 0; j < groups[i].Count; j++) {
                        groups[i][j].Start();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the group coordinator associated with this scene.
        /// </summary>
        public BehaviorGroupCoordinator Coordinator
        {
            get
            {
                return coordinator;
            }
            set
            {
                if (coordinator != value) {
                    coordinator = value;

                    // fire changed event
                }
            }
        }
    }
}

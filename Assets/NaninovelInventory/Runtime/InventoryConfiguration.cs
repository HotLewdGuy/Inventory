using Naninovel;
using UnityEngine;

namespace NaninovelInventory
{
    /// <summary>
    /// Contains configuration data for the inventory-related systems.
    /// </summary>
    [System.Serializable]
    public class InventoryConfiguration : Configuration
    {
        /// <summary>
        /// Used to distinguish inventory-related records among other resources.
        /// </summary>
        public const string DefaultPathPrefix = "Inventory";

        [Tooltip("Configuration of the resource loader used with inventory resources.")]
        public ResourceLoaderConfiguration Loader = new ResourceLoaderConfiguration { PathPrefix = DefaultPathPrefix };
    }
}

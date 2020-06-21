using Naninovel;
using System.Collections.Generic;
using UniRx.Async;

namespace NaninovelInventory
{
    /// <summary>
    /// A custom engine service used to manage available inventory items.
    /// </summary>
    [InitializeAtRuntime] // makes the service auto-initialize with other built-in engine services
    [Naninovel.Commands.Goto.DontReset] // skips the service reset on @goto commands (to preserve the inventory when navigating scripts)
    public class InventoryManager : IEngineService<InventoryConfiguration>
    {
        public InventoryConfiguration Configuration { get; }

        /// <summary>
        /// Stores (caches) loaded item prefabs in `Item ID -> Item Prefab` map.
        /// </summary>
        private readonly Dictionary<string, InventoryItem> loadedItems = new Dictionary<string, InventoryItem>();
        private readonly IResourceProviderManager providersManager;
        private ResourceLoader<InventoryItem> itemLoader;

        public InventoryManager (InventoryConfiguration config, IResourceProviderManager providersManager)
        {
            // Engine service constructors are invoked when the engine is initializing;
            // remember that it's not safe to use other services here, as they are not initialized yet.
            // Instead, store references to the required services and use them in `InitializeServiceAsync()` method.

            Configuration = config;
            this.providersManager = providersManager; // required to load item prefabs
        }

        public UniTask InitializeServiceAsync ()
        {
            // Invoked when the engine is initializing, after services required in the constructor are initialized;
            // it's safe to use the required services here (IResourceProviderManager in this case).

            // Initialize item prefab loader, as per the configuration.
            itemLoader = Configuration.Loader.CreateFor<InventoryItem>(providersManager);

            return UniTask.CompletedTask;
        }

        public void ResetService ()
        {
            // Invoked when resetting engine state (eg, loading a script or starting a new game).

            var inventory = Engine.GetService<IUIManager>().GetUI<InventoryUI>();
            if (ObjectUtils.IsValid(inventory))
                inventory.RemoveAllItems(); // remove all items from the current inventory

            itemLoader?.UnloadAll(); // unload item prefabs to free memory
            loadedItems.Clear(); // clear cached items
        }

        public void DestroyService ()
        {
            // Invoked when destroying the engine.

            itemLoader?.UnloadAll();
            loadedItems.Clear();
        }

        /// <summary>
        /// Attempts to retrieve item prefab with the specified identifier.
        /// </summary>
        public async UniTask<InventoryItem> GetItemAsync (string itemId)
        {
            // If item prefab is already loaded, return it.
            if (loadedItems.TryGetValue(itemId, out var item))
                return item;

            // Attempt to load the required item.
            item = await itemLoader.LoadAsync(itemId);

            // If failed to load the item, return null.
            if (!ObjectUtils.IsValid(item)) return null;

            // Make sure item name is equal to the requested ID.
            item.name = itemId;

            // Cache the loaded item prefab in the map by its ID.
            loadedItems[itemId] = item;

            return item;
        }
    }
}

using Naninovel;
using Naninovel.UI;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace NaninovelInventory
{
    /// <summary>
    /// Component applied to the root of inventory UI prefab.
    /// </summary>
    public class InventoryUI : CustomUI
    {
        /// <summary>
        /// Represents serializable inventory state.
        /// More info about using custom state in Nainovel: https://naninovel.com/guide/state-management.html#custom-state
        /// </summary>
        [System.Serializable]
        private new class GameState
        {
            public Vector3 Position;
            public List<InventorySlotState> OccupiedSlots;
        }

        [Tooltip("Number of slots the inventory has.")]
        [SerializeField] private int capacity = 80;
        [Tooltip("Reference to the inventory grid layout component.")]
        [SerializeField] private InventoryGrid grid = default;
        [Tooltip("Content of the inventory.")]
        [SerializeField] private RectTransform content = default;

        private InventoryManager inventoryManager;
        private IInputManager inputManager;

        /// <summary>
        /// Returns number of items with the provided ID currently assigned
        /// to the inventory slots.
        /// </summary>
        /// <param name="itemId">Identifier of the item to count.</param>
        public int ItemCount (string itemId) => grid.GetAllSlots().Sum(s => s.ItemAssigned && s.Item.Id == itemId ? s.StackCount : 0);

        /// <summary>
        /// Attempts to add an item with the provided <paramref name="itemId"/> to the first 
        /// empty inventory slot, or slot with the same item, when stack limit allows.
        /// </summary>
        /// <param name="itemId">Identifier of the item to add.</param>
        /// <param name="amount">Number of items to add.</param>
        /// <returns>Whether the item(s) were added.</returns>
        public async UniTask<bool> AddItemAsync (string itemId, int amount = 1)
        {
            // Check if we can stack the items.
            var stackSlot = grid.FindSlot(s => s.ItemAssigned && s.Item.Id == itemId && s.Item.StackCountLimit >= (s.StackCount + amount));
            if (ObjectUtils.IsValid(stackSlot))
                return await AddItemAtAsync(itemId, stackSlot.Id, amount);

            // In case can't stack, find first empty slot.
            var emptySlot = grid.FindSlot(s => !s.ItemAssigned);
            if (!ObjectUtils.IsValid(emptySlot))
            {
                Debug.LogError($"Failed to add `{itemId}`: no empty slots available.");
                return false;
            }

            return await AddItemAtAsync(itemId, emptySlot.Id, amount);
        }

        /// <summary>
        /// Attempts to add an item with the provided <paramref name="itemId"/> to an inventory 
        /// slot with the provided <paramref name="slotId"/>.
        /// </summary>
        /// <param name="itemId">Identifier of the item to add.</param>
        /// <param name="slotId">Identifier of an inventory slot to put the item into.</param>
        /// <param name="amount">Number of items to add.</param>
        /// <returns>Whether the item(s) were added.</returns>
        public async UniTask<bool> AddItemAtAsync (string itemId, string slotId, int amount = 1)
        {
            var slot = grid.GetSlot(slotId);
            if (!ObjectUtils.IsValid(slot))
            {
                Debug.LogError($"Failed to add `{itemId}` to `{slotId}` slot: slot with the provided ID doesn't exist.");
                return false;
            }

            var item = await inventoryManager.GetItemAsync(itemId);
            if (!ObjectUtils.IsValid(item))
            {
                Debug.LogError($"Failed to add `{itemId}` to `{slotId}` slot: item with the provided ID doesn't exist.");
                return false;
            }

            if (slot.ItemAssigned && slot.Item.Id != itemId)
            {
                Debug.LogError($"Failed to add `{itemId}` to `{slotId}` slot: slot is already occupied with `{slot.Item.Id}` item.");
                return false;
            }

            if (slot.ItemAssigned && (slot.StackCount + amount) > item.StackCountLimit)
            {
                Debug.LogError($"Failed to add `{itemId}` (x{amount}) to `{slotId}` slot: exceeding stack count limit.");
                return false;
            }

            if (!slot.ItemAssigned)
                slot.AssignItem(item);
            slot.SetStackCount(slot.StackCount + amount);

            return true;
        }

        /// <summary>
        /// Attempts to remove item with the provided <paramref name="itemId"/> from the inventory.
        /// </summary>
        /// <param name="itemId">Identifier of the item to remove.</param>
        /// <param name="amount">Number of items to remove.</param>
        /// <returns>Whether the item was removed.</returns>
        public bool RemoveItem (string itemId, int amount = 1)
        {
            while (amount > 0)
            {
                var slot = grid.FindSlot(s => s.ItemAssigned && s.Item.Id == itemId);
                if (!ObjectUtils.IsValid(slot))
                {
                    Debug.LogError($"Failed to remove `{itemId}` item (x{amount}): item is not added to the inventory or not enough stacks.");
                    return false;
                }

                var removeCount = Mathf.Min(amount, slot.StackCount);
                RemoveItemAt(slot.Id, removeCount);
                amount -= removeCount;
            }
            return true;
        }

        /// <summary>
        /// Attempts to remove an item from an inventory slot with the provided <paramref name="slotId"/>.
        /// </summary>
        /// <param name="slotId">Identifier of the inventory slot to remove item from.</param>
        /// <param name="amount">Number of items to remove.</param>
        /// <returns>Whether the item was removed.</returns>
        public bool RemoveItemAt (string slotId, int amount = 1)
        {
            var slot = grid.GetSlot(slotId);
            if (!ObjectUtils.IsValid(slot))
            {
                Debug.LogError($"Failed to remove an item from `{slotId}` slot: slot with the provided ID doesn't exist.");
                return false;
            }

            if (!slot.ItemAssigned)
            {
                Debug.LogError($"Failed to remove an item from `{slotId}` slot: no item is assigned to the slot.");
                return false;
            }

            if (slot.StackCount < amount)
            {
                Debug.LogError($"Failed to remove `{slot.Item.Id}` item (x{amount}) from `{slotId}` slot: not enough stacks.");
                return false;
            }

            slot.SetStackCount(slot.StackCount - amount);
            if (slot.StackCount == 0) slot.RemoveItem();
            return true;
        }

        /// <summary>
        /// Removes all the items assigned to inventory slots.
        /// </summary>
        public void RemoveAllItems () => grid.GetAllSlots().ForEach(s => s.RemoveItem());

        /// <summary>
        /// Attempts to use an item with the provided <paramref name="itemId"/>.
        /// </summary>
        /// <param name="itemId">Identifier of the item to use.</param>
        /// <returns>Whether the item was found and used.</returns>
        public bool UseItem (string itemId)
        {
            var slot = grid.FindSlot(s => s.ItemAssigned && s.Item.Id == itemId);
            if (!ObjectUtils.IsValid(slot))
            {
                Debug.LogError($"Failed to use `{itemId}` item: item doesn't exist in inventory.");
                return false;
            }

            slot.Item.Use(slot.Id);

            return true;
        }

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(grid, content); // make sure the required objects are assigned in the inspector

            // Store reference to the engine services -- we'll need them later.
            inventoryManager = Engine.GetService<InventoryManager>();
            inputManager = Engine.GetService<IInputManager>();

            // Add the required amount of empty slots to the grid.
            for (int i = 0; i < capacity; i++)
            {
                var slotId = i.ToString(); // using grid cell index as slot identifier
                var slotProto = grid.SlotPrototype;
                var slot = new ScriptableGridSlot.Constructor<InventoryGridSlot>(slotProto, slotId, HandleSlotClicked).ConstructedSlot;
                grid.AddSlot(slot);
            }
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            // Start listening for `ToggleInventory` input event to toggle UI's visibility.
            var toggleSampler = inputManager.GetSampler("ToggleInventory");
            if (toggleSampler != null)
                toggleSampler.OnStart += ToggleVisibility;
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            // Stop listening for `ToggleInventory` input event.
            var toggleSampler = inputManager?.GetSampler("ToggleInventory");
            if (toggleSampler != null)
                toggleSampler.OnStart -= ToggleVisibility;
        }

        protected virtual void HandleSlotClicked (string id)
        {
            var slot = grid.GetSlot(id);
            if (!slot.ItemAssigned) return;

            slot.Item.Use(id);
        }

        protected override void SerializeState (GameStateMap stateMap)
        {
            // Invoked when the game is saved.

            base.SerializeState(stateMap);

            // Serialize UI state.
            var state = new GameState() {
                Position = content.transform.position,
                OccupiedSlots = grid.GetAllSlots()                                      // get all slots in the inventory
                    .Where(s => s.ItemAssigned)                                         // filter-out empty slots
                    .Select(s => new InventorySlotState(s.Id, s.Item.Id, s.StackCount)) // create state structs for non-empty slots
                    .ToList()                                                           // create list collection from the query result
            };
            stateMap.SetState(state);
        }

        protected override async UniTask DeserializeState (GameStateMap stateMap)
        {
            // Invoked when the game is loaded.

            await base.DeserializeState(stateMap);

            RemoveAllItems();

            var state = stateMap.GetState<GameState>();
            if (state is null) return; // empty state, do nothing

            // Restore UI state.
            if (state.OccupiedSlots?.Count > 0)
            {
                var tasks = state.OccupiedSlots.Select(s => AddItemAtAsync(s.ItemId, s.SlotId, s.StackCount));
                await UniTask.WhenAll(tasks);
            }
            content.transform.position = state.Position;
        }
    }
}

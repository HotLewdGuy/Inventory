using System;
using System.Collections.Generic;
using UnityEngine;

namespace NaninovelInventory
{
    /// <summary>
    /// Represents serializable state of an (occupied) inventory slot.
    /// </summary>
    [System.Serializable]
    public struct InventorySlotState : IEquatable<InventorySlotState>
    {
        /// <summary>
        /// Identifier of the inventory slot.
        /// </summary>
        public string SlotId => slotId;
        /// <summary>
        /// Identifier of an item in the slot.
        /// </summary>
        public string ItemId => itemId;
        /// <summary>
        /// Number of items stacked in the inventory slot.
        /// </summary>
        public int StackCount => stackCount;

        [SerializeField] private string slotId;
        [SerializeField] private string itemId;
        [SerializeField] private int stackCount;

        public InventorySlotState (string slotId, string itemId, int stackCount)
        {
            this.slotId = slotId;
            this.itemId = itemId;
            this.stackCount = stackCount;
        }

        // Below are the methods required by IEquatable<> interface.
        // The interface is implemented for performance reasons and is not mandatory for the inventory example;
        // more info: https://devblogs.microsoft.com/premier-developer/performance-implications-of-default-struct-equality-in-c/

        public override bool Equals (object obj) => obj is InventorySlotState state && Equals(state);

        public bool Equals (InventorySlotState other) => itemId == other.itemId && slotId == other.slotId;

        public override int GetHashCode ()
        {
            int hashCode = -1607297225;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(itemId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(slotId);
            return hashCode;
        }
    }
}

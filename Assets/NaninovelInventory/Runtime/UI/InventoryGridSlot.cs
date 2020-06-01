using Naninovel;
using UnityEngine;
using UnityEngine.UI;

namespace NaninovelInventory
{
    /// <summary>
    /// Applied to the root of inventory slot prefab.
    /// </summary>
    public class InventoryGridSlot : ScriptableGridSlot
    {
        /// <summary>
        /// Item that is currently assigned to the slot.
        /// </summary>
        public InventoryItem Item { get; private set; }
        /// <summary>
        /// Whether an item is currently assigned to the slot.
        /// </summary>
        public bool ItemAssigned => ObjectUtils.IsValid(Item);
        /// <summary>
        /// Number of <see cref="Item"/> assigned to the slot.
        /// </summary>
        public int StackCount { get; private set; }

        [Tooltip("Text to show current stack count of the item (if the slot is occupied).")]
        [SerializeField] private Text stackCountText = default;

        /// <summary>
        /// Assigns provided <paramref name="item"/> to the slot.
        /// </summary>
        /// <param name="item">The item to assign.</param>
        public void AssignItem (InventoryItem item)
        {
            if (ItemAssigned)
            {
                Debug.LogError($"Failed to assign `{item.Id}` item to `{Id}` slot: `{item.Id}` item is already assigned to this slot.");
                return;
            }

            Item = Instantiate(item);
            Item.name = item.Id;

            Item.transform.SetParent(transform, false);
            Item.transform.SetAsFirstSibling(); // this will make stack count text render on top of the item
        }

        /// <summary>
        /// Un-assigns currently assigned item.
        /// </summary>
        public void RemoveItem ()
        {
            if (ItemAssigned)
            {
                ObjectUtils.DestroyOrImmediate(Item.gameObject);
                Item = null;
            }
            SetStackCount(0);
        }

        /// <summary>
        /// Modifies <see cref="StackCount"/>.
        /// </summary>
        public void SetStackCount (int count)
        {
            StackCount = count;

            stackCountText.text = count.ToString();
            stackCountText.gameObject.SetActive(count > 1);
        }

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(stackCountText); // make sure required objects are assigned in the inspector

            stackCountText.gameObject.SetActive(false); // initially hide the stack count text
        }
    }
}

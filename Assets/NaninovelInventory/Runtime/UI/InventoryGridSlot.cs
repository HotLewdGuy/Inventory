using Naninovel;
using UnityEngine;
using UnityEngine.UI;

namespace NaninovelInventory
{
    /// <summary>
    /// Attached to the root of inventory slot prefab.
    /// </summary>
    public class InventoryGridSlot : ScriptableGridSlot
    {
        public override string Id => BindSlot?.Id.ToString();
        public InventorySlot BindSlot { get; private set; }

        [Tooltip("Text to show current stack count of the item (if the slot is occupied).")]
        [SerializeField] private Text stackCountText = default;

        private InventoryItem item;

        public void Bind (InventorySlot inventorySlot)
        {
            if (BindSlot != null)
            {
                BindSlot.OnItemChanged -= SetItem;
                BindSlot.OnStackCountChanged -= SetStackCount;
            }

            BindSlot = inventorySlot;
            BindSlot.OnItemChanged += SetItem;
            BindSlot.OnStackCountChanged += SetStackCount;

            SetItem(inventorySlot.Item);
            SetStackCount(inventorySlot.StackCount);
        }

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(stackCountText); // make sure required objects are assigned in the inspector
            stackCountText.gameObject.SetActive(false); // initially hide the stack count text
        }

        private void SetItem (InventoryItem prototype)
        {
            if (ObjectUtils.IsValid(prototype))
            {
                // Note that this naive implementation is just for example.
                // In real projects use object pool instead of instantiating and destroying the items.
                if (item) ObjectUtils.DestroyOrImmediate(item.gameObject);
                item = Instantiate(prototype, transform, false);
                item.transform.SetAsFirstSibling(); // this will make stack count text render on top of the item
            }
            else SetEmpty();
        }

        private void SetStackCount (int count)
        {
            stackCountText.text = count.ToString();
            stackCountText.gameObject.SetActive(count > 1);
        }

        private void SetEmpty ()
        {
            if (item) ObjectUtils.DestroyOrImmediate(item.gameObject);
            SetStackCount(0);
        }
    }
}

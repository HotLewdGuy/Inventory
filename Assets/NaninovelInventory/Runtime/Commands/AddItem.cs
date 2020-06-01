using Naninovel;
using Naninovel.Commands;
using System.Threading;
using UniRx.Async;

namespace NaninovelInventory
{
    /// <summary>
    /// Adds item with the specified identifier to the inventory.
    /// When slot ID is provided, will assign item to the slot; otherwise will attempt to use first empty slot.
    /// </summary>
    public class AddItem : Command
    {
        /// <summary>
        /// Identifier of the item to add.
        /// </summary>
        [RequiredParameter, ParameterAlias(NamelessParameterAlias)]
        public StringParameter ItemId;
        /// <summary>
        /// Identifier of the slot for which to assign the item.
        /// </summary>
        public StringParameter SlotId;
        /// <summary>
        /// Number of items to add.
        /// </summary>
        public IntegerParameter Amount = 1;

        public override async UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            if (Assigned(SlotId))
                await inventory.AddItemAtAsync(ItemId, SlotId, Amount);
            else await inventory.AddItemAsync(ItemId, Amount);
        }
    }
}
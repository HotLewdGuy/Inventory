using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

namespace NaninovelInventory
{
    [Documentation("Adds item with the specified identifier to the inventory. When slot ID is provided, will assign item to the slot; otherwise will attempt to use first empty slot.")]
    public class AddItem : Command
    {
        [RequiredParameter, ParameterAlias(NamelessParameterAlias), Documentation("Identifier of the item to add.")]
        public StringParameter ItemId;
        [Documentation("Identifier of the slot for which to assign the item.")]
        public StringParameter SlotId;
        [Documentation("Number of items to add.")]
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
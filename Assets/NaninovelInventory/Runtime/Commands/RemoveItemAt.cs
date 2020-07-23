using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

namespace NaninovelInventory
{
    [Documentation("Removes item from an inventory slot with the specified identifier.")]
    public class RemoveItemAt : Command
    {
        [RequiredParameter, ParameterAlias(NamelessParameterAlias), Documentation("Identifier of inventory slot to remove item from.")]
        public StringParameter SlotId;
        [Documentation("Number of items to remove.")]
        public IntegerParameter Amount = 1;

        public override UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            inventory.RemoveItemAt(SlotId, Amount);

            return UniTask.CompletedTask;
        }
    }
}
using Naninovel;
using Naninovel.Commands;
using System.Threading;
using UniRx.Async;

namespace NaninovelInventory
{
    /// <summary>
    /// Removes item from an inventory slot with the specified identifier.
    /// </summary>
    public class RemoveItemAt : Command
    {
        /// <summary>
        /// Identifier of inventory slot to remove item from.
        /// </summary>
        [RequiredParameter, ParameterAlias(NamelessParameterAlias)]
        public StringParameter SlotId;
        /// <summary>
        /// Number of items to remove.
        /// </summary>
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
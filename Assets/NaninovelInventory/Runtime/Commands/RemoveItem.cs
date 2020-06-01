using Naninovel;
using Naninovel.Commands;
using System.Threading;
using UniRx.Async;

namespace NaninovelInventory
{
    /// <summary>
    /// Removes item with the specified identifier from the inventory.
    /// </summary>
    public class RemoveItem : Command
    {
        /// <summary>
        /// Identifier of the item to remove.
        /// </summary>
        [RequiredParameter, ParameterAlias(NamelessParameterAlias)]
        public StringParameter ItemId;
        /// <summary>
        /// Number of items to remove.
        /// </summary>
        public IntegerParameter Amount = 1;

        public override UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            inventory.RemoveItem(ItemId, Amount);

            return UniTask.CompletedTask;
        }
    }
}
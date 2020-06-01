using Naninovel;
using Naninovel.Commands;
using System.Threading;
using UniRx.Async;

namespace NaninovelInventory
{
    /// <summary>
    /// Uses item with the specified identifier (in case it's exist in the inventory).
    /// </summary>
    public class UseItem : Command
    {
        /// <summary>
        /// Identifier of the item to use.
        /// </summary>
        [RequiredParameter, ParameterAlias(NamelessParameterAlias)]
        public StringParameter ItemId;

        public override UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            inventory.UseItem(ItemId);

            return UniTask.CompletedTask;
        }
    }
}
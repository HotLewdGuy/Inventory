using Naninovel;
using Naninovel.Commands;
using System.Threading;
using UniRx.Async;

namespace NaninovelInventory
{
    /// <summary>
    /// Removes all item in the inventory.
    /// </summary>
    public class RemoveAllItems : Command
    {
        public override UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            inventory.RemoveAllItems();

            return UniTask.CompletedTask;
        }
    }
}
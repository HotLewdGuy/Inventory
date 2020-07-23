using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

namespace NaninovelInventory
{
    [Documentation("Removes all item in the inventory.")]
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
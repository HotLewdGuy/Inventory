using Naninovel;
using Naninovel.Commands;
using UniRx.Async;

namespace NaninovelInventory
{
    [Documentation("Uses item with the specified identifier (in case it's exist in the inventory).")]
    public class UseItem : Command
    {
        [RequiredParameter, ParameterAlias(NamelessParameterAlias), Documentation("Identifier of the item to use.")]
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
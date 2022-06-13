using System;
using System.Linq;
namespace WidgetScmDataAccess
{
    public class Inventory
    {
        private ScmContext context;
        public Inventory(ScmContext context)
        {
            this.context = context;
        }

        public void UpdateInventory() 
        {
            foreach (var cmd in context.GetPartCommands())
            {
                var item = context.Inventory.Single(i => i.PartTypeId == cmd.PartTypeId);
                if(cmd.Command == PartCountOperation.Add)
                    item.Count += cmd.PartCount;
                else
                    item.Count -= cmd.PartCount;

                context.UpdateInventoryItem(item.PartTypeId, item.Count);
                context.DeletePartCommand(cmd.Id);
            }
        }
    }
}
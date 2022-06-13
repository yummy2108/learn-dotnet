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
                var oldCount = item.Count;
                if(cmd.Command == PartCountOperation.Add)
                    item.Count += cmd.PartCount;
                else
                    item.Count -= cmd.PartCount;
                // transaction ACID
                var transaction = context.BeginTransaction();
                try{
                    context.UpdateInventoryItem(item.PartTypeId, item.Count, transaction);
                    context.DeletePartCommand(cmd.Id, transaction);
                    transaction.Commit();
                }
                catch{
                    transaction.Rollback();
                    item.Count = oldCount;
                    throw;
                }
                
            }
        }
    }
}
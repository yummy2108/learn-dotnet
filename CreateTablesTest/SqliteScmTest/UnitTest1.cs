using System;
using Xunit;
using WidgetScmDataAccess;
using System.Linq;

namespace SqliteScmTest
{
    public class UnitTest1 : IClassFixture<SampleScmDataFixture>
    {
        private SampleScmDataFixture fixture;
        private ScmContext context;

        public UnitTest1(SampleScmDataFixture fixture)
        {
            this.fixture = fixture;
            this.context = new ScmContext(fixture.Connection);
        }

        [Fact]
        public void Test1()
        {
            var parts = context.Parts;
            Assert.Equal(1, parts.Count());
            var part = parts.First();
            Assert.Equal("8289 L-shaped plate", part.Name);

            var inventory = context.Inventory;
            Assert.Equal(1, inventory.Count());
            var item = inventory.First();
            Assert.Equal(part.Id, item.PartTypeId);
            Assert.Equal(100, item.Count);
            Assert.Equal(10, item.OrderThreshold);
        }

        [Fact]
        public void TestPartCommands()
        {
            var item = context.Inventory.First();
            var startCount = item.Count;
            context.CreatePartCommand(new PartCommand(){
                PartTypeId = item.PartTypeId,
                PartCount = 10,
                Command = PartCountOperation.Add
            });

            context.CreatePartCommand(new PartCommand(){
                PartTypeId = item.PartTypeId,
                PartCount = 5,
                Command = PartCountOperation.Remove
            });

            var inventory = new Inventory(context);
            inventory.UpdateInventory();
            Assert.Equal(startCount + 5, item.Count);
        }
    }
}

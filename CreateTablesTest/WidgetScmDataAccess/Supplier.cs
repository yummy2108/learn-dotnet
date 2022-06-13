using System;

namespace WidgetScmDataAccess
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int PartTypeId { get; set; }
        public PartType Part { get; set; }
    }
}
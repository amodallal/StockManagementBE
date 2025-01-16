using System.Collections.Generic;

namespace StockManagement
{
    public class AddItemCapacitiesRequest
    {
        public int ItemId { get; set; } // The ID of the item to associate with capacities
        public List<int> CapacityIds { get; set; } // A list of Capacity IDs to associate with the item
    }
}
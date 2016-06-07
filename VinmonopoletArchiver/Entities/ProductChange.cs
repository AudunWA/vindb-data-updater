using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinmonopoletArchiver.Entities
{
    /// <summary>
    /// Represents a change of a product field.
    /// </summary>
    public class ProductChange
    {
        public long ProductID { get; set; }
        public ProductField ChangedField { get; set; }
        public DateTime ChangeTime { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public ProductChange(long productID, ProductField changedField, DateTime changeTime, string oldValue, string newValue)
        {
            ProductID = productID;
            ChangedField = changedField;
            ChangeTime = changeTime;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            return $"ProductID: {ProductID}, ChangedField: {ChangedField}, OldValue: {OldValue}, NewValue: {NewValue}";
        }
    }
}

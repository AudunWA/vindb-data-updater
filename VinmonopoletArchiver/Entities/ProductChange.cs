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
    internal class ProductChange
    {
        public int ProductID { get; set; }
        public string ChangedField { get; set; }
        public DateTime ChangeTime { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}

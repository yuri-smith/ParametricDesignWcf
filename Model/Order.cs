using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public int OrderID { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int SellerCompanyID { get; set; }
        [DataMember]
        public int CustomerCompanyID { get; set; }

        public virtual Company Seller { get; set; }
        public virtual Company Customer { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}

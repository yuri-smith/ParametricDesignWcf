using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class SellerCustomerCompany
    {
        [DataMember]
        [ForeignKey("Seller"), Column(Order = 1)]
        public int SellerCompanyID { get; set; }
        [DataMember]
        [ForeignKey("Customer"), Column(Order = 2)]
        public int CustomerCompanyID { get; set; }
        [DataMember]
        public int? Discount { get; set; }

        [DataMember]
        public virtual Company Seller { get; set; }
        [DataMember]
        public virtual Company Customer { get; set; }

        [NotMapped]
        [DataMember]
        public string SellerName { get; set; }
    }
}

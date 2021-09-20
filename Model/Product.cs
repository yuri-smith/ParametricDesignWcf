using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public string Name { get; set; }

        public int OrderOrderID { get; set; }
        public int TypeProductTypeProductID { get; set; }

        public virtual Order Order { get; set; }
        public virtual TypeProduct TypeProduct { get; set; }
    }
}

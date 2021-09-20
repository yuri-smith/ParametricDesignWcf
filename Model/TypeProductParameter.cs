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
    public class TypeProductParameter
    {
        [DataMember]
        [ForeignKey("TypeProduct"), Column(Order = 1)]
        public int TypeProductTypeProductID { get; set; }
        [DataMember]
        [ForeignKey("Parameter"), Column(Order = 2)]
        public int ParameterParameterID { get; set; }
        [DataMember]
        public int DefaultValue { get; set; }

        public virtual TypeProduct TypeProduct { get; set; }
        [DataMember]
        public virtual Parameter Parameter { get; set; }

        [NotMapped]
        [DataMember]
        public string ParameterName { get; set; }

    }
}

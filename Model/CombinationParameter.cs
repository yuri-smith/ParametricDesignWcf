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
    public class CombinationParameter
    {
        [DataMember]
        [ForeignKey("Combination"), Column(Order = 1)]
        public int CombinationCombinationID { get; set; }
        [DataMember]
        [ForeignKey("Parameter"), Column(Order = 2)]
        public int ParameterParameterID { get; set; }
        [DataMember]
        public int MinValue { get; set; }
        [DataMember]
        public int MaxValue { get; set; }

        public virtual Combination Combination { get; set; }
        public virtual Parameter Parameter { get; set; }
    }
}

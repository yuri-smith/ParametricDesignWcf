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
    public class NodeDetailFitting
    {
        [DataMember]
        [ForeignKey("Node"), Column(Order = 1)]
        public int NodeFittingID { get; set; }
        [DataMember]
        [ForeignKey("Detail"), Column(Order = 2)]
        public int DetailFittingID { get; set; }
        [DataMember]
        public int Count { get; set; }

        public virtual Fitting Node { get; set; }
        public virtual Fitting Detail { get; set; }
    }
}

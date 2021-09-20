using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Dim
    {
        [DataMember]
        public int DimID { get; set; }
        [DataMember]
        public string Name { get; set; }

        public virtual ICollection<Fitting> Fittings { get; set; }
        public virtual ICollection<CombinationFitting> DimCounts { get; set; }
        public virtual ICollection<CombinationFitting> DimSizes { get; set; }
    }
}

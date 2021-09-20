using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Combination
    {
        [DataMember]
        public int CombinationID { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int TypeProductTypeProductID { get; set; }

        public virtual TypeProduct TypeProduct { get; set; }
        public virtual ICollection<CombinationParameter> CombinationParameters { get; set; }
        public virtual ICollection<CombinationFitting> CombinationFittings { get; set; }

    }
}

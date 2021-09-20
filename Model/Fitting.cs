using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Fitting
    {
        [DataMember]
        public int FittingID { get; set; }
        [DataMember]
        public string Article { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public int CompanyCompanyID { get; set; }
        [DataMember]
        public int DimCountDimID { get; set; }

        [DataMember]
        public virtual Company Company { get; set; }
        [DataMember]
        public virtual Dim DimCount { get; set; }

        [DataMember]
        public virtual ICollection<CombinationFitting> CombinationFittings { get; set; }

        [DataMember]
        public virtual ICollection<NodeDetailFitting> Nodes { get; set; }
        [DataMember]
        public virtual ICollection<NodeDetailFitting> Details { get; set; }

        [NotMapped]
        [DataMember]
        public string FullName { get; set; }
    }
}

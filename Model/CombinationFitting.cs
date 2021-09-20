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
    public class CombinationFitting
    {
        [DataMember]
        public int CombinationFittingID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Size { get; set; }
        [DataMember]
        public string Count { get; set; }
        [DataMember]
        public int CombinationCombinationID { get; set; }
        [DataMember]
        public int FittingFittingID { get; set; }
        [DataMember]
        public int DimCountDimID { get; set; }
        [DataMember]
        public int? DimSizeDimID { get; set; }


        [DataMember]
        public virtual Combination Combination { get; set; }
        [DataMember]
        public virtual Fitting Fitting { get; set; }
        [DataMember]
        public virtual Dim DimCount { get; set; }
        [DataMember]
        public virtual Dim DimSize { get; set; }

        [NotMapped]
        [DataMember]
        public string FittingArticle { get; set; }

    }
}

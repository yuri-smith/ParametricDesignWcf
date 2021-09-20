using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Parameter
    {
        [DataMember]
        public int ParameterID { get; set; }
        [DataMember]
        public string Name { get; set; }
        //[DataMember]
        //public int AccountAccountID { get; set; }

        //public virtual Account Account { get; set; }
        public virtual ICollection<TypeProductParameter> TypeProductParameters { get; set; }
        public virtual ICollection<CombinationParameter> CombinationParameters { get; set; }
    }
}

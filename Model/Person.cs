using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Person
    {
        [DataMember]
        public int PersonID { get; set; }
        [DataMember]
        public string Name { get; set; }

        //[DataMember]
        public int CompanyCompanyID { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}

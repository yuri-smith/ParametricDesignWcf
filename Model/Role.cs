using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Role
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}

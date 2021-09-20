using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public int AccountID { get; set; }
        [DataMember]
        [MinLength(8, ErrorMessage="Логин должен содержать не менее 8-ми символов")]
        public string Login { get; set; }
        [DataMember]
        [MinLength(8, ErrorMessage = "Пароль должен содержать не менее 8-ми символов")]
        public string Password { get; set; }

        public int? PersonPersonID { get; set; }

        public virtual Person Person { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        //public virtual ICollection<Parameter> Parameters { get; set; }
        public virtual ICollection<TypeProduct> TypeProduct { get; set; }
    }
}

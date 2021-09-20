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
    public class TypeProduct
    {
        [DataMember]
        public Int32 TypeProductID { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Не может быть пустой строкой!")]
        public string Name { get; set; }

        [DataMember]
        [Required]
        public int AccountAccountID { get; set; }
        [DataMember]
        public int? ParentTypeProductID { get; set; }

        public virtual Account Account { get; set; }
        public virtual TypeProduct Parent { get; set; }

        public virtual ICollection<TypeProduct> ChildTypeProducts { get; set; }
        public virtual ICollection<TypeProductParameter> TypeProductParameters { get; set; }
        public virtual ICollection<Combination> Combinations { get; set; }

    }
}

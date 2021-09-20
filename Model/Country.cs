using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Country
    {
        [DataMember]
        public int CountryID { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public virtual ICollection<Region> Regions { get; set; }
        [DataMember]
        public virtual ICollection<City> Cities { get; set; }

    }
}

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
    public class Region
    {
        [DataMember]
        public int RegionID { get; set; }
        [DataMember]
        public string Name { get; set; }
        //[DataMember]
        //public int CountryCountryID { get; set; }

        [DataMember]
        //public virtual Country Country { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        [DataMember]
        public virtual ICollection<Country> Countries { get; set; }

        [NotMapped]
        [DataMember]
        public string FullName { get; set; }
        //How do I expose non persisted properties using a WCF Data Service - CodeProject.htm
        //http://www.codeproject.com/Questions/119229/How-do-I-expose-non-persisted-properties-using-a-W

        [NotMapped]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullNameFromDB
        {
            get
            {
                return (Countries != null && Countries.Count > 0) ? Name + ", " + Countries.First().Name : Name;
            }
        }
    }
}

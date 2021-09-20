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
    public class City
    {
        [DataMember]
        public int CityID { get; set; }
        //[DataMember]
        //public int? CountryCountryID { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        //public virtual Country Country { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
        [DataMember]
        public virtual ICollection<Country> Countries { get; set; }

        public virtual ICollection<Company> LegalCompanies { get; set; }
        public virtual ICollection<Company> ActualCompanies { get; set; }

        [NotMapped]
        [DataMember]
        public string FullName { get; set; }

        [NotMapped]
        //[DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullNameFromDB
        {
            get
            { 
                return (Countries != null && Countries.Count > 0) ? Name + ", " + 
                    Countries.First().Name : Name; 
            }
        }

    }
}

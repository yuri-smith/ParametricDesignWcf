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
    public class Company
    {
        public Company()
        {
            this.LegalAddress = new Address();
            this.ActualAddress = new Address();
        }

        [DataMember]
        public int CompanyID { get; set; }
        [DataMember]
        public string INN { get; set; }
        [DataMember]
        public string KPP { get; set; }
        [DataMember]
        public string LongName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public DateTime? DatePrice { get; set; }
        [DataMember]
        public int LegalCityCityID { get; set; }
        [DataMember]
        public int? ActualCityCityID { get; set; }
        [DataMember]
        public Address LegalAddress { get; set; }
        [DataMember]
        public Address ActualAddress { get; set; }
        [DataMember]
        public virtual City LegalCity { get; set; }
        [DataMember]
        public virtual City ActualCity { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
        public virtual ICollection<Fitting> Fittings { get; set; }
        public virtual ICollection<Order> AsSellerOrders { get; set; }
        public virtual ICollection<Order> AsCustomerOrders { get; set; }
        [DataMember]
        public virtual ICollection<SellerCustomerCompany> Sellers { get; set; }
        [DataMember]
        public virtual ICollection<SellerCustomerCompany> Customers { get; set; }

        [NotMapped]
        [DataMember]
        public string FullName { get; set; }


        [NotMapped]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullNameFromDB
        {
            get
            {
                return (LegalCity != null) ? Name + ", " + LegalCity.FullNameFromDB : Name;
            }
        }
    }
}

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
    public class Address
    {
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string House { get; set; }
        [DataMember]
        public string Office { get; set; }

        //public string FullName
        //{
        //    get { return City.ToString() + ", " + Street + ", " + House + ", " + Office; }
        //}
    }
}

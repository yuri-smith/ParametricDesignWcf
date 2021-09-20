using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    public class Session
    {
        public Guid SessionID { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateClose { get; set; }

        public int AccountAccountID { get; set; }
        public virtual Account Account { get; set; }
    }
}

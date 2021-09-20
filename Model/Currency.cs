using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ParametricDesignWcfServiceLibrary.Model
{
    [DataContract]
    public class Currency
    {
        [DataMember]
        public int CurrencyID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Curs { get; set; }
        [DataMember]
        public DateTime DateCurs { get; set; }
        [DataMember]
        public virtual ICollection<Company> Companies { get; set; }
        //http://www.cyberforum.ru/windows-forms/thread941874.html
        //кросс-курс https://otvet.mail.ru/question/25570659
        //курс конкретной валюты http://kbyte.ru/ru/Programming/Sources.aspx?id=919&mode=show
        //тоже http://www.csharpcoderr.com/2012/09/kurs-valut.html
    }
}

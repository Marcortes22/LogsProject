using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Collections
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        
        public string Provincia { get; set; }

        public int Canton { get; set; }


        public int CreditCard { get; set; }
    }
}

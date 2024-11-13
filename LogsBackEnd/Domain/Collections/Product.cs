using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Collections
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int Count { get; set; }  


        public string Category { get; set; }


        public int CommerceID { get; set; }


    }
}

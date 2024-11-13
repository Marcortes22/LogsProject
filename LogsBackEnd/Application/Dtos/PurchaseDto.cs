using Domain.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class PurchaseDto
    {

        public int ID { get; set; }

        public required List<Product> Details { get; set; } = new List<Product>();


        public required double Total { get; set; } = 0.0;

        public required User User { get; set; }


        public bool IsSuccess { get; set; }


        public required DateTime Time { get; set; } = DateTime.UtcNow;


    }
}

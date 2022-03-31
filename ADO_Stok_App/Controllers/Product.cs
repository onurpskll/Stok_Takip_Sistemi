using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_Stok_App.Controllers
{
    public abstract class Product
    {
        public int id { get; set; }
        public int price { get; set; }
        public int type { get; set; }
    }
}

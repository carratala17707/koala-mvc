using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Koala.Models
{
    public class CartViewModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public double Descuento { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Koala.Models
{
    public class CartViewModel
    {
        public List<CartViewModel> Cart { get; set; }

        public int IdCompra { get; set; }
        public int IdLineaPedido { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public double Descuento { get; set; }
    }
}
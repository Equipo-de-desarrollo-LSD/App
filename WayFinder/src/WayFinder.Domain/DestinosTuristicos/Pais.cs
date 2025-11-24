using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WayFinder.Calificaciones
{
    public class Pais :ValueObject
    {
        public string nombre { get; set; }
        public double poblacion { get; set; }
        public static readonly Pais Empty = new Pais();

        private Pais() { }
        public Pais(string nombre, double poblacion)
        {
            this.nombre = nombre;
            this.poblacion = poblacion;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return nombre;
            yield return poblacion;
        }

    }
}

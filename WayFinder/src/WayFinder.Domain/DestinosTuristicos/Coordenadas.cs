using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;


namespace WayFinder.DestinosTuristicos
{
    public class Coordenadas : ValueObject
    {
        public double latitud { get; set; }
        public double longitud { get; set; }
        public static readonly Coordenadas Empty = new Coordenadas();

        private Coordenadas() { }  
        public Coordenadas(double latitud, double longitud)
        {
            this.latitud = latitud;
            this.longitud = longitud;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return latitud;
            yield return longitud;
        }

    }
}

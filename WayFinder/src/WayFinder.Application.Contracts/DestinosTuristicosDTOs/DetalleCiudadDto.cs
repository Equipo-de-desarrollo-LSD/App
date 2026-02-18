using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayFinder.DestinosTuristicosDTOs
{
        public class DetalleCiudadDto
        {
            // IDs para referencias futuras
            public string GeoDbId { get; set; }
            public string WikiDataId { get; set; }

            // Información General
            public string Nombre { get; set; }
            public string Pais { get; set; }      
            public string Region { get; set; }     // Provincia o Estado (ej: "Entre Ríos")
            public int Poblacion { get; set; }
            public CoordenadasDto Coordenadas { get; set; }

            // Información Detallada 
            public string ZonaHoraria { get; set; }      // Ej: "America/Argentina/Cordoba"
            public double? ElevacionMetros { get; set; } // Altura sobre el nivel del mar
        }
    }
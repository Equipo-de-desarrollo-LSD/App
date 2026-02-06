using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using WayFinder.DestinosTuristicos;

namespace WayFinder.DestinosTuristicos
{
    public class DestinoTuristico : Entity<Guid>
    {
        /* public int Id { get; set; }
         public required string nombre { get; set; }
         public required string foto { get; set; }
         public DateTime UltimaActualizacion { get; set; }
         public Pais pais { get; set; }
         public Coordenadas coordenadas { get; set; }
        */
        //public DestinoTuristico(Guid id) { Id = id; }
        //  [Key] public Guid Id { get; set; }
        public required string nombre { get; set; }
        public required string foto { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public Pais Pais { get; set; }
        public Coordenadas Coordenadas {get; set;}
    

    

    public DestinoTuristico(Guid id) : base(id)
        {
            // Constructor vacío necesario para que ABP pueda asignar el ID
        }
    }
}

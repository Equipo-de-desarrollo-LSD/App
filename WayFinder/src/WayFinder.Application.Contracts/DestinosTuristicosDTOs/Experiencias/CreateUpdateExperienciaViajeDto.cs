using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WayFinder.DestinosTuristicos;

namespace WayFinder.DestinosTuristicos
{
    public class CreateUpdateExperienciaViajeDto
    {
        [Required]
        public Guid DestinoTuristicoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        public string Contenido { get; set; }

        [Required]
        public TipoExperiencia Sentimiento { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayFinder.DestinosTuristicosDTOs
{
    public class CalificacionDto
    {
        public string? Comentario { get; set; }

        public int Puntaje { get; set; }

        public Guid DestinoId { get; set; }
        public Guid UserId { get; set; }
    }
    public class CrearCalificacionDto
    {
        //[Required]
        public Guid DestinoId { get; set; }

        //[Range(1, 5)] // Para la prueba de puntuación válida
        public int Puntaje { get; set; }

        public string? Comentario { get; set; }
        public Guid UserId { get; set; }

    }
}

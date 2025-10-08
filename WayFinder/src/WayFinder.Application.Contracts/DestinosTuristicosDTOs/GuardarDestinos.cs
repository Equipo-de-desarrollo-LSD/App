using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WayFinder.DestinosTuristicosDTOs;

public class GuardarDestinos
{
      [Required]
      [StringLength(128)] 
      public string nombre { get; set; } = string.Empty;

    //      [Range(1, int.MaxValue, ErrorMessage = "El id debe ser mayor que cero.")]
      public Guid Id { get; set; } 

      [Required]
      [StringLength(256)]
      public string foto { get; set; } = string.Empty;
      public DateTime ultimaActualizacion { get; set; } = DateTime.Now;

      [Required]
      public PaisDto pais { get; set; } = new PaisDto();

      [Required]
      public CoordenadasDto coordenadas { get; set; } = new CoordenadasDto();
 
}





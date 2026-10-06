using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayFinder.DestinosTuristicosDTOs.Perfiles
{
    public class PerfilPublicoDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        // Datos extra
        public string? Foto { get; set; }
        public string? Preferencias { get; set; }

        // NO ponemos el Email aquí por privacidad 🛡️
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using WayFinder.DestinosTuristicosDTOs.Perfiles;

namespace WayFinder.Perfiles
{
    public interface IPerfilAppService : IApplicationService
    // En este servicio, solo se permiten operaciones relacionadas con MI perfil,
    // no con los perfiles de otros usuarios
    {
        // Método para obtener MIS datos
        Task<PerfilDto> GetMiPerfilAsync();

        // Método para guardar cambios en MI perfil
        Task UpdateMiPerfilAsync(ActualizarPerfilDto input);

        // Método para eliminar MI perfil 
        Task EliminarMiCuentaAsync();

        // Consultar el perfil de alguien más usando su ID
        Task<PerfilPublicoDto> GetPerfilPublicoAsync(Guid id);
    }
}
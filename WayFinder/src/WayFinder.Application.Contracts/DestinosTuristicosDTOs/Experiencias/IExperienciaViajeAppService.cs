using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WayFinder.DestinosTuristicos
{
    public interface IExperienciaViajeAppService :
        ICrudAppService<
            ExperienciaViajeDto,
            Guid,
            GetExperienciasInput,
            CreateUpdateExperienciaViajeDto>
    {
        // Al heredar de ICrudAppService, ya incluimos automáticamente:
        // GetAsync, GetListAsync, CreateAsync, UpdateAsync, DeleteAsync
    }
}
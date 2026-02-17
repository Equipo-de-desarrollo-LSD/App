using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using WayFinder.DestinosTuristicos;

namespace WayFinder.DestinosTuristicos
{
    public class ExperienciaViajeAppService :
        CrudAppService<
            ExperienciaViaje,
            ExperienciaViajeDto,
            Guid,
            GetExperienciasInput,
            CreateUpdateExperienciaViajeDto>,
        IExperienciaViajeAppService
    {
        public ExperienciaViajeAppService(IRepository<ExperienciaViaje, Guid> repository)
            : base(repository)
        {
        }
        
        public override async Task<ExperienciaViajeDto> UpdateAsync(Guid id, CreateUpdateExperienciaViajeDto input)
        {
            var entity = await Repository.GetAsync(id);
            if (entity.CreatorId != CurrentUser.Id)
            {
                throw new UserFriendlyException("Solo puedes editar tus propias experiencias.");
            }
            return await base.UpdateAsync(id, input);
        }

        public override async Task<ExperienciaViajeDto> CreateAsync(CreateUpdateExperienciaViajeDto input)
        {
            var nuevaExperiencia = new ExperienciaViaje()
            {
                DestinoTuristicoId = input.DestinoTuristicoId,
                Titulo = input.Titulo,
                Contenido = input.Contenido,
                Sentimiento = input.Sentimiento
            };

            // 2. Insertar en Base de Datos
            await Repository.InsertAsync(nuevaExperiencia);

            // 3. Devolver DTO (El mapeo de salida SÍ funciona bien)
            return ObjectMapper.Map<ExperienciaViaje, ExperienciaViajeDto>(nuevaExperiencia);
        }

        public override async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            if (entity.CreatorId != CurrentUser.Id)
            {
                throw new UserFriendlyException("Solo puedes eliminar tus propias experiencias.");
            }
            await base.DeleteAsync(id);
        }
    
    protected override async Task<IQueryable<ExperienciaViaje>> CreateFilteredQueryAsync(GetExperienciasInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            return query
                .Where(x => x.CreatorId == CurrentUser.Id) 
                .WhereIf(input.DestinoTuristicoId.HasValue, x => x.DestinoTuristicoId == input.DestinoTuristicoId)
                .WhereIf(input.Sentimiento.HasValue, x => x.Sentimiento == input.Sentimiento)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                     x => x.Titulo.Contains(input.Filter) || x.Contenido.Contains(input.Filter));
        }
    }
}
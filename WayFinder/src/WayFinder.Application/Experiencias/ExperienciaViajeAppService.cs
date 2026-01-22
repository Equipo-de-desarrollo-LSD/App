using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

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

        // ✅ Lógica para FILTROS (Puntos 4.5 y 4.6)
        protected override async Task<IQueryable<ExperienciaViaje>> CreateFilteredQueryAsync(GetExperienciasInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            return query
                .WhereIf(input.DestinoTuristicoId.HasValue, x => x.DestinoTuristicoId == input.DestinoTuristicoId)
                .WhereIf(input.Sentimiento.HasValue, x => x.Sentimiento == input.Sentimiento)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Titulo.Contains(input.Filter) || x.Contenido.Contains(input.Filter));
        }

        // ✅ Lógica de SEGURIDAD (Puntos 4.2 y 4.3 - Solo el dueño edita)
        public override async Task<ExperienciaViajeDto> UpdateAsync(Guid id, CreateUpdateExperienciaViajeDto input)
        {
            var entity = await Repository.GetAsync(id);
            if (entity.CreatorId != CurrentUser.Id)
            {
                throw new UserFriendlyException("Solo puedes editar tus propias experiencias.");
            }
            return await base.UpdateAsync(id, input);
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
    }
}
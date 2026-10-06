using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WayFinder.MetricasDTOs;

namespace WayFinder.Metricas
{
    [Authorize(Roles = "admin")] // Solo Admin puede consultar
    public class MetricaAppService : WayFinderAppService, IMetricaAppService
    {
        private readonly IRepository<MetricaApi, Guid> _metricaRepository;

        public MetricaAppService(IRepository<MetricaApi, Guid> metricaRepository)
        {
            _metricaRepository = metricaRepository;
        }

        public async Task<List<MetricaDto>> GetListAsync()
        {
            var metricas = await _metricaRepository.GetListAsync();
            return metricas.Select(x => new MetricaDto
            {
                NombreServicio = x.NombreServicio,
                Endpoint = x.Endpoint,
                TiempoRespuestaMs = x.TiempoRespuestaMs,
                FechaEjecucion = x.FechaEjecucion
            }).ToList();
        }
    }
}

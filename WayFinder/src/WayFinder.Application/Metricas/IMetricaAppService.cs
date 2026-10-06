using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using WayFinder.MetricasDTOs;

namespace WayFinder.Metricas
{
    public interface IMetricaAppService : IApplicationService
    {
        Task<List<MetricaDto>> GetListAsync();
    }
}

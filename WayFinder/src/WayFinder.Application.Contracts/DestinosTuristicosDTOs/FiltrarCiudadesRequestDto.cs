using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WayFinder.DestinosTuristicosDTOs
{
    public class FiltrarCiudadesRequestDto
    {
        // El código del país según norma ISO (Ej: "AR" para Argentina, "US" para EEUU)
        // Es opcional (string?) porque quizás solo quieran filtrar por población.
        public string? PaisCodigo { get; set; }

        // La población mínima que buscan (Ej: 100000)
        // Es opcional (int?) porque quizás solo quieran filtrar por país.
        public int? MinPoblacion { get; set; }

        // Cantidad de resultados a traer (por defecto 5)
        public int Limit { get; set; } = 5;
    }
}
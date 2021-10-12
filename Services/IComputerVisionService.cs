using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MisVacaciones.Services.Dto;

namespace MisVacaciones.Services
{
    public interface ICompuerVisionService
    {
        Task<ImageAnalysisViewModel> AnalyzeImageUrl(string imageUrl);
    }
}

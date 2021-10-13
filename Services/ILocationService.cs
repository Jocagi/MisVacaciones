using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MisVacaciones.Services.Dto;

namespace MisVacaciones.Services
{
    interface ILocationService
    {
        public Task<CoordinatesViewModel> AnalyzeAddress(string location);
    }
}

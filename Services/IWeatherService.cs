using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MisVacaciones.Services.Dto;
using QuickType;

namespace MisVacaciones.Services
{
    interface IWeatherService
    {
        public Task<Weather> GetWeather(double longitude, double latitude);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleMaps.LocationServices;
using MisVacaciones.Services.Dto;

namespace MisVacaciones.Services
{
    public class LocationService : ILocationService
    { 
        private const string apiKey = "AIzaSyABjlTQ4n7GEdS3gErCOgGiirYFAelOMeA";
        
        public async Task<CoordinatesViewModel> AnalyzeAddress(string location)
        {
            CoordinatesViewModel coordinates = new CoordinatesViewModel();
            try
            {
                var locationService = new GoogleLocationService(apiKey);
                var point = locationService.GetLatLongFromAddress(location);

                coordinates.latitude = point.Latitude;
                coordinates.longitude = point.Longitude;
                return coordinates;

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

using MisVacaciones.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickType;
using System.Net;
using System.IO;

namespace MisVacaciones.Services
{
    public class WeatherService : IWeatherService
    {
        private const string apiKey = "cc29f7360691daf1a08416de131c00ca";
        private const string forecastDays = "16";
        private const string measureUnits = "metric";
        private const string endpoint = "https://api.openweathermap.org/data/2.5/forecast/daily?lat={lat}&lon={lon}&units={units}&cnt={cnt}&appid={API key}";

        public async Task<Weather> GetWeather(double longitude, double latitude) 
        {
            //Reemplazar parámetros
            string responseJSON = string.Empty;
            string url = endpoint;
            url = url.Replace("{lat}", latitude.ToString());
            url = url.Replace("{lon}", longitude.ToString());
            url = url.Replace("{cnt}", forecastDays);
            url = url.Replace("{units}", measureUnits);
            url = url.Replace("{API key}", apiKey);

            //Enviar solicitud a API
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            //Leer respuesta
            using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseJSON = await reader.ReadToEndAsync();
            }

            //Convertir JSON a objeto
            var weather = Weather.FromJson(responseJSON);

            return weather;
        }
    }
}

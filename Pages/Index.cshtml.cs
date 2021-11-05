using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MisVacaciones.Services;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MisVacaciones.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IComputerVisionService _computerVisionService;
        private readonly ILocationService _locationService;
        private readonly IWeatherService _weatherService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _computerVisionService = new ComputerVisionService();
            _locationService = new LocationService();
            _weatherService = new WeatherService();
            this._hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public FileUpload fileUpload { get; set; }
        public void OnGet()
        {
            ViewData["SuccessMessage"] = "";
        }
        public async Task<IActionResult> OnPostUpload(FileUpload fileUpload)
        {
            string fullPath = _hostEnvironment.WebRootPath + "/UploadImages/";
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            var formFile = fileUpload.FormFile;
            if (formFile.Length > 0)
            {
                var filePath = Path.Combine(fullPath, formFile.FileName);
                ViewData["ImageUrl"] = formFile.FileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                // using service to analyze the image  
                var imageAnalysis = await _computerVisionService.AnalyzeImageUrl(filePath);
                if (imageAnalysis.imageAnalysisResult != null)
                {
                    ViewData["ImageAnalysisViewModel"] = imageAnalysis;
                    
                    // using service to get location 
                    if (imageAnalysis.imageAnalysisResult.Categories.Count >= 1 &&
                        imageAnalysis.imageAnalysisResult.Categories[0].Detail != null &&
                        imageAnalysis.imageAnalysisResult.Categories[0].Detail.Landmarks.Count >= 1)
                    {
                        var location = imageAnalysis.imageAnalysisResult.Categories[0].Detail.Landmarks[0].Name;
                        if (location != null)
                        {
                            var coordinates = await _locationService.AnalyzeAddress(location);
                            if (coordinates != null) 
                            {
                                ViewData["Coordinates"] = coordinates;

                                //using service to get weather
                                var weather = await _weatherService.GetWeather(coordinates.longitude, coordinates.latitude);

                                if (weather != null) 
                                {
                                    ViewData["Weather"] = weather;
                                }
                            }
                        }
                    }
                }
            }

            ViewData["SuccessMessage"] = fileUpload.FormFile.FileName.ToString() + " file uploaded!!";


            return Page();
        }
        public void OnPostEnviar(string correo, string cuerpo, string hostname)
        {
            cuerpo = replaceSRC(cuerpo, hostname);
            string EmailOrigen = "bruteforce.misvacaciones@gmail.com";
            string EmailDestion = correo;
            string contraseña = "123genesis";
            string Cuerpo = cuerpo;
            MailMessage mailMessage = new MailMessage(EmailOrigen, EmailDestion, "Mis vacaciones: información de ubicación", Cuerpo);
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, contraseña);
            smtpClient.Send(mailMessage);
            smtpClient.Dispose();
        }

        private string replaceSRC(string html, string host) 
        {
            html = html.Replace("UploadImages", host + "/UploadImages");
            html = html.Replace("assets", host + "/assets");


            html = html.Replace("\n", " ");
            html = html.Replace("\t", " ");
            html = Regex.Replace(html, "\\s+", " ");
            html = Regex.Replace(html, "<head.*?</head>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = Regex.Replace(html, "<script.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = Regex.Replace(html, "<form.*?</form>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = Regex.Replace(html, "<header.*?</header>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            return html;
        }

        public class FileUpload
        {
            [Required]
            [Display(Name = "File")]
            public IFormFile FormFile { get; set; }
            public string SuccessMessage { get; set; }
        }


    }
}

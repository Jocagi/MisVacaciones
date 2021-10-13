﻿using Microsoft.AspNetCore.Mvc;
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

namespace MisVacaciones.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IComputerVisionService _computerVisionService;
        private readonly ILocationService _locationService;
        private readonly IWebHostEnvironment _hostEnvironment;
        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _computerVisionService = new ComputerVisionService();
            _locationService = new LocationService();
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
                    ViewData["ImageAnalysisViewModel"] = imageAnalysis;
                // using service to get location 
                var location = imageAnalysis.imageAnalysisResult.Categories[0].Detail.Landmarks[0].Name;
                var coordinates = await _locationService.AnalyzeAddress(location);
                if (coordinates != null)
                    ViewData["Coordinates"] = coordinates;
            }

            ViewData["SuccessMessage"] = fileUpload.FormFile.FileName.ToString() + " file uploaded!!";


            return Page();
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
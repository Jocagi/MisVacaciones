using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace MisVacaciones.Services.Dto
{
    public class ImageAnalysisViewModel
    {
        public ImageAnalysis imageAnalysisResult { get; set; }
    }
}
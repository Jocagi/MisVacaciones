using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using MisVacaciones.Services.Dto;

namespace MisVacaciones.Services
{
    public class ComputerVisionService : IComputerVisionService
    {
        // Add your Computer Vision subscription key and endpoint    
        private const string subscriptionKey = "0ad494e269314a85a3b092c0cf561eb4";
        private const string endpoint = "https://imageanalysisurl.cognitiveservices.azure.com/";
        /*  
         * AUTHENTICATE  
         * Creates a Computer Vision client used by each example.  
         */
        public ComputerVisionClient Authenticate()
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = endpoint
            };
            return client;
        }
        public async Task<ImageAnalysisViewModel> AnalyzeImageUrl(string imageUrl)
        {
            try
            {
                //Authentication
                ComputerVisionClient client = Authenticate();
                // Creating a list that defines the features to be extracted from the image.
                List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>() 
                {
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                    VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                    VisualFeatureTypes.Objects
                };
                // Creating a list that defines the details to be extracted from the image.
                List<Details?> details = new List<Details?>()
                {
                    Details.Landmarks, Details.Celebrities
                };
                //Results
                ImageAnalysis results;
                using (Stream imageStream = File.OpenRead(imageUrl))
                {
                    results = await client.AnalyzeImageInStreamAsync(imageStream, visualFeatures: features, details: details);
                    //imageStream.Close();    
                }
                ImageAnalysisViewModel imageAnalysis = new ImageAnalysisViewModel();
                imageAnalysis.imageAnalysisResult = results;
                return imageAnalysis;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);    
                throw;
            }
        }
    }
}

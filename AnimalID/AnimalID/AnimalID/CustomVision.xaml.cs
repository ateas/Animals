using Plugin.Geolocator;
using AnimalID.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AnimalID.DataModels;

namespace AnimalID
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void LoadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
            await postLocationAsync();
            

            await MakePredictionRequest(file);
        }

        async Task postLocationAsync()
        {

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

            Animalinformation model = new Animalinformation()
            {
               
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude

            };

            await AzureManager.AzureManagerInstance.PostAnimalInformation(model);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "d06c74beccc74d5e8f36f5c5bf2bc284");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/63e30bb6-0363-490a-b6c9-9be8bde7c44a/image?iterationId=659c7227-1380-4e63-b7a1-3815e9a2e43e";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);
                    int max = 0;
                    for(int i = 0; i != responseModel.Predictions.Count; i++)
                    {
                        max = responseModel.Predictions[i].Probability >= responseModel.Predictions[max].Probability ? i : max;
                    }
                    
                    
                    TagLabel.Text = (responseModel.Predictions[max].Probability >= 0.5) ? "This image most probably contains a " + responseModel.Predictions[max].Tag + ". We say this with a probability of " + responseModel.Predictions[max].Probability : "Sorry we cannot identify that animal";
                }
            }

        }

    }
}

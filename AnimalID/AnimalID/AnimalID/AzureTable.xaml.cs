using AnimalID.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AnimalID
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzureTable : ContentPage
    {
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;
        public AzureTable()
        {
            InitializeComponent();

        }
        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            loader.IsRunning = true;
            loader.IsVisible = true;
            List<Animalinformation> Animalyoyo = await AzureManager.AzureManagerInstance.Getmyanimals();

            AnimalList.ItemsSource = Animalyoyo;
            loader.IsRunning = false;
            loader.IsVisible = false;
            
        }

       

    }
    

}

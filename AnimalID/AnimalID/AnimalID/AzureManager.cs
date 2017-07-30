using AnimalID.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalID
{
    
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Animalinformation> AnimalTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://myanimals.azurewebsites.net");
            this.AnimalTable = this.client.GetTable<Animalinformation>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
              
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<Animalinformation>> Getmyanimals()
        {
            return await this.AnimalTable.ToListAsync();
        }

        public async Task PostAnimalInformation(Animalinformation Animalinformation)
        {
            await this.AnimalTable.InsertAsync(Animalinformation);
        }

        public async Task UpdateAnimalInformation(Animalinformation Animalinformation)
        {
            await this.AnimalTable.UpdateAsync(Animalinformation);
        }

        public async Task DeleteAnimalInformation(Animalinformation Animalinformation)
        {
            await this.AnimalTable.DeleteAsync(Animalinformation);
        }
    }
}

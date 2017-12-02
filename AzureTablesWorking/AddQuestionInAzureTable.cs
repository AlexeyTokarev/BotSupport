using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTablesWorking
{
    public static class AddQuestionInAzureTable
    {
        private static string _partitionKey;
        private static string _rowKey;
        public static void AddData(string platform, string role, string question, string answer, string channel)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Answers");
            table.CreateIfNotExists();

            Guid firstGuid = Guid.NewGuid();
            _partitionKey = firstGuid.ToString();

            Guid secondGuid = Guid.NewGuid();
            _rowKey = secondGuid.ToString();
            
            CustomerEntity data = new CustomerEntity(_partitionKey, _rowKey)
            {
                Platform = platform,
                Role = CorrectRole(platform, role),
                Question = question,
                Answer = answer,
                DateAndTime = ShowMoskowTime(),
                IsCorrect = "Не определено",
                Channel = channel,
                Device = string.Empty
            };

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(data);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        public static void UpdateData(string platform, string role, string question, string answer, string channel, bool isCorrect)
        {
            string correctToString = isCorrect ? "true" : "false";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Answers");
            table.CreateIfNotExists();
            CustomerEntity data = new CustomerEntity(_partitionKey, _rowKey)
            {
                Platform = platform,
                Role = CorrectRole(platform, role),
                Question = question,
                Answer = answer,
                DateAndTime = ShowMoskowTime(),
                IsCorrect = correctToString,
                Channel = channel,
                Device = string.Empty,
            };

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(data);
            table.Execute(insertOrReplaceOperation);
            //// Create the TableOperation object that inserts the customer entity.
            //TableOperation insertOperation = TableOperation.Insert(data);

            //// Execute the insert operation.
            //table.Execute(insertOperation);
        }

        private static string ShowMoskowTime()
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            string moskowDate = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
            return moskowDate;
        }
        public static string CorrectRole(string defaultPlatform, string defaultRole)
        {
            string role;
            switch (defaultPlatform)
            {
                case "223-ФЗ":
                    switch (defaultRole)
                    {
                        case "Заказчик": role = defaultRole; break;
                        case "Поставщик": role = "Участник"; break;
                        default: role = defaultRole; break;
                    }
                    break;

                case "44-ФЗ":
                    switch (defaultRole)
                    {
                        case "Заказчик": role = defaultRole; break;
                        case "Поставщик": role = "Участник"; break;
                        default: role = defaultRole; break;
                    }
                    break;

                case "615-ПП РФ":
                    switch (defaultRole)
                    {
                        case "Заказчик": role = defaultRole; break;
                        case "Поставщик": role = "Участник"; break;
                        case "ОВР": role = defaultRole; break;
                        default: role = defaultRole; break;
                    }
                    break;

                case "Имущественные торги":
                    switch (defaultRole)
                    {
                        case "Продавец": role = "Продавец/Арендодатель"; break;
                        case "Покупатель": role = "Претендент/Арендатор"; break;
                        default: role = defaultRole; break;
                    }
                    break;

                case "РТС-Маркет":
                    switch (defaultRole)
                    {
                        case "Заказчик": role = defaultRole; break;
                        case "Поставщик": role = defaultRole; break;
                        default: role = defaultRole; break;
                    }
                    break;

                default: role = defaultRole; break;

            }

            return role;
        }
    }
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public CustomerEntity() { }

        public string Platform { get; set; }
        public string Role { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string DateAndTime { get; set; }
        public string IsCorrect { get; set; }
        public string Channel { get; set; }
        public string Device { get; set; }
    }


}

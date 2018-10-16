using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GoogleTablesWorking
{
    public class AddQuestionInGoogleSheet
    {
        private static readonly string KeyDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "key.p12");
        private static readonly string AppName = "TestTable";
        private static string SpreadsheetId;
        private static string[] Data = new string[5];

        public static void SendError(string platform, string role, string userQuestion, string answer, bool correct)
        {
            if (!correct) SpreadsheetId = "1B_qS-3HzAZ4zTQkrCjgVHBXzo_D89DcX_TWmVILahCw";
            if (correct) SpreadsheetId = "10MCeGzO9D5bjtkwvH5R0oeDA0hxJj2AlXggbUqlNYuE";

            Data[0] = platform;
            Data[1] = CorrectRole(platform, role);
            Data[2] = userQuestion;
            Data[3] = answer;
            Data[4] = ShowMoskowTime();

            String serviceAccountEmail = "tessheet3@curious-domain-178413.iam.gserviceaccount.com";

            var certificate = new X509Certificate2(KeyDirectory, "notasecret",
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            var xml = certificate.PrivateKey.ToXmlString(true);

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xml);

            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = new[] { SheetsService.Scope.Spreadsheets },
                    Key = rsa
                });

            // Create the service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });

            FillSpreadsheet(service, SpreadsheetId, Data);
        }

        /// <summary>
        /// Метод реализует заполнение таблицы данными
        /// </summary>
        /// <param name="service"></param>
        /// <param name="spreadsheetId"></param>
        /// <param name="data"></param>
        private static void FillSpreadsheet(SheetsService service, string spreadsheetId, string[] data)
        {
            // Нахождение свободной десятки
            var diapason = Task.Run(() => Diapason(service, spreadsheetId));
            var tenner = diapason.Result;

            // Нахождение свободной единицы
            var task = Task.Run(() => FindFreeRow(service, spreadsheetId, tenner));
            var row = task.Result;


            List<Request> requests = new List<Request>();
            List<CellData> values = new List<CellData>();


            foreach (var a in data)
            {
                values.Add(new CellData
                {
                    UserEnteredValue = new ExtendedValue { StringValue = a }
                });
            }

            requests.Add(new Request
            {
                UpdateCells = new UpdateCellsRequest
                {
                    Start = new GridCoordinate
                    {
                        SheetId = 0,
                        RowIndex = row - 1,
                        ColumnIndex = 0
                    },
                    Rows = new List<RowData>
                    {
                        new RowData { Values = values }
                    },
                    Fields = "userEnteredValue"
                }
            });


            BatchUpdateSpreadsheetRequest busr = new BatchUpdateSpreadsheetRequest
            {
                Requests = requests
            };

            service.Spreadsheets.BatchUpdate(busr, spreadsheetId).Execute();
        }

        /// <summary>
        /// Метод реализует проверку на наличие пустой строки для удобства записи
        /// </summary>
        /// <param name="service"></param>
        /// <param name="spreadsheetId"></param>
        /// <param name="tenner"></param>
        /// <returns></returns>
        private static async Task<int> FindFreeRow(SheetsService service, string spreadsheetId, int tenner)
        {
            SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum valueRenderOption = 0;
            SpreadsheetsResource.ValuesResource.GetRequest.DateTimeRenderOptionEnum dateTimeRenderOption = 0;

            int rowNumber = 0;
            int firstcount = 1;

            if (tenner < 10)
            {
                firstcount = tenner;
            }
            else firstcount = tenner - 10;

            int secondcount = firstcount + 10;

            for (rowNumber = firstcount; rowNumber < secondcount; rowNumber++)
            {
                string range = $"A{rowNumber}";

                SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
                request.ValueRenderOption = valueRenderOption;
                request.DateTimeRenderOption = dateTimeRenderOption;
                try
                {
                    ValueRange response = await request.ExecuteAsync();

                    var jsonobj = JsonConvert.SerializeObject(response);
                    dynamic obj = JsonConvert.DeserializeObject(jsonobj);

                    var values = obj.values;

                    if (values != null)
                    {
                        if (rowNumber == secondcount - 1)
                        {
                            firstcount += 10;
                            secondcount += 10;
                        }
                    }
                    if (values == null)
                    {
                        return rowNumber;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return rowNumber;
        }

        private static async Task<int> Diapason(SheetsService service, string spreadsheetId)
        {
            SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum valueRenderOption = 0;
            SpreadsheetsResource.ValuesResource.GetRequest.DateTimeRenderOptionEnum dateTimeRenderOption = 0;


            int tenner = 0;
            int firstcount = 1;
            int secondcount = 100;

            for (tenner = firstcount; tenner < secondcount; tenner += 10)
            {
                string range = $"A{tenner}";

                SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
                request.ValueRenderOption = valueRenderOption;
                request.DateTimeRenderOption = dateTimeRenderOption;
                try
                {
                    ValueRange response = await request.ExecuteAsync();

                    var jsonobj = JsonConvert.SerializeObject(response);
                    dynamic obj = JsonConvert.DeserializeObject(jsonobj);

                    var values = obj.values;

                    if (values != null)
                    {
                        if (tenner == secondcount - 9)
                        {
                            firstcount += 100;
                            secondcount += 100;
                        }
                    }
                    if (values == null)
                    {
                        return tenner;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return tenner;
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

                case "Электронный магазин ЗМО":
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
}

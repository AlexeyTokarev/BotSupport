using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json;

namespace GoogleTablesWorking
{
    public class AddQuestionInGoogleSheet
    {
        private static readonly string ClientSecret = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "client_secret.json");
        private static readonly string[] ScopesSheets = { SheetsService.Scope.Spreadsheets };
        private const string AppName = "TestTable";
        private const string SpreadsheetId = "1B_qS-3HzAZ4zTQkrCjgVHBXzo_D89DcX_TWmVILahCw";

        private static string[] Data = new string[5];


        public static string SendError(string platform, string role, string userQuestion, string answer)
        {
            try
            {
                Data[0] = platform;
                Data[1] = role;
                Data[2] = userQuestion;
                Data[3] = answer;
                Data[4] = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss (UTC zzz)");

                var credential = GetSheetCredentials();
                var service = GetService(credential);
                FillSpreadsheet(service, SpreadsheetId, Data);
                return
                    "Спасибо большое. Ваше мнение очень важно для нас. В ближайшее время наша служба техподдержки рассмотрит Ваш ответ. Приносим извинения за неудобство.";
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        private static UserCredential GetSheetCredentials()
        {
            using (var srteam = new FileStream(ClientSecret, FileMode.Open, FileAccess.Read))
            {
                var creadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "sheetsCreds.json");
                return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(srteam).Secrets,
                    ScopesSheets, "user", CancellationToken.None, new FileDataStore(creadPath, true)).Result;
            }
        }

        private static SheetsService GetService(UserCredential credential)
        {
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName
            });
        }

        /// <summary>
        /// Метод реализует заполнение таблицы данными
        /// </summary>
        /// <param name="service"></param>
        /// <param name="spreadsheetId"></param>
        /// <param name="data"></param>
        private static void FillSpreadsheet(SheetsService service, string spreadsheetId, string[] data)
        {
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
                        RowIndex = FindFreeRow(service, spreadsheetId),
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
        /// <returns></returns>
        private static int FindFreeRow(SheetsService service, string spreadsheetId)
        {
            SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum valueRenderOption = 0;
            SpreadsheetsResource.ValuesResource.GetRequest.DateTimeRenderOptionEnum dateTimeRenderOption = 0;

            bool emptyRow = false;
            int rowNumber = 1;

            while (emptyRow == false)
            {
                string range = $"A{rowNumber}:E{rowNumber}";

                SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
                request.ValueRenderOption = valueRenderOption;
                request.DateTimeRenderOption = dateTimeRenderOption;

                ValueRange response = request.Execute();
                var jsonobj = JsonConvert.SerializeObject(response);
                dynamic obj = JsonConvert.DeserializeObject(jsonobj);
                var values = obj.values;

                if (values != null)
                {
                    rowNumber++;
                    continue;
                }

                emptyRow = true;
            }
            return rowNumber - 1;
        }
    }
}
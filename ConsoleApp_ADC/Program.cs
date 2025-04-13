using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using HIS_DB_Lib;
using Basic;


namespace ConsoleApp_ADC
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = @"C:\HIS"; // 修改為你的檔案路徑
            //string folderPath = logDirectory;

            string searchPattern = "*.txt";
            string NewFile = null;
            List<string> todayFiles = new List<string>();

            string[] files = Directory.GetFiles(folderPath, searchPattern, System.IO.SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                NewFile = files.OrderBy(f => Path.GetFileName(f)).Last();
            }
            string filePath = Path.Combine(folderPath, NewFile);

            List<DrugRecord> drugRecords = new List<DrugRecord>();
            List<batch_inventory_exportClass> batch_Inventory_ExportClasses = new List<batch_inventory_exportClass>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("Big5")))
            {
                string line;
                string currentWard = "";  // 儲存當前護理站名稱
                string[] alertKeywords = { "胰島", "化療", "抗凝", "KCL" };

                while ((line = reader.ReadLine()) != null)
                {

                    // 抓護理站
                    if (line.Contains("護理站："))
                    {
                        var match = Regex.Match(line, @"護理站：(\d+)\s+(.+)");
                        if (match.Success)
                        {
                            currentWard = match.Groups[1].Value;
                        }
                    }

                    // 判斷是否為藥品資料列（根據位置解析）
                    if (string.IsNullOrWhiteSpace(line) || line.Contains("衛生福利部") || line.Contains("非藥包機藥品") || line.Contains("護理站") || line.StartsWith(" 劑") || line.StartsWith(" ===")) continue;

                    // 將每行依據多個空格切割成欄位
                    var tokens = Regex.Split(line.Trim(), @"\s{1,}");

                    if (tokens.Length < 3) continue;

                    try
                    {
                        string dosage = "";
                        string alert = "";
                        string code = "";
                        string name = "";
                        int quantity = 0;

                        // 如果第一欄是劑型
                        if (tokens[0].Contains("劑") || tokens[0].Contains("口服"))
                        {
                            dosage = tokens[0];

                            // 判斷第二欄是否為高警
                            if (alertKeywords.Any(k => tokens[1].Contains(k)))
                            {
                                alert = tokens[1];
                                code = tokens[2];
                                name = string.Join(" ", tokens.Skip(3).Take(tokens.Length - 4));
                            }
                            else
                            {
                                alert = "";
                                code = tokens[1];
                                name = string.Join(" ", tokens.Skip(2).Take(tokens.Length - 3));
                            }
                        }
                        else
                        {
                            // 沒有劑型的行
                            if (alertKeywords.Any(k => tokens[0].Contains(k)))
                            {
                                alert = tokens[0];
                                code = tokens[1];
                                name = string.Join(" ", tokens.Skip(2).Take(tokens.Length - 3));
                            }
                            else
                            {
                                alert = "";
                                code = tokens[0];
                                name = string.Join(" ", tokens.Skip(1).Take(tokens.Length - 2));
                            }
                        }

                        // 最後一欄是數量
                        int.TryParse(tokens[tokens.Length - 1], out quantity);

                        // 加入結果
                        drugRecords.Add(new DrugRecord
                        {
                            Ward = currentWard,
                            Code = code,
                            Name = name.Trim(),
                            Quantity = quantity
                        });
                        batch_Inventory_ExportClasses.Add(new batch_inventory_exportClass
                        {
                            藥碼 = code,
                            數量 = quantity.ToString(),
                            效期 = DateTime.MaxValue.ToDateTimeString(),
                        }) ;
                        //Console.WriteLine($"{code}")
                        
                    }
                    catch
                    {
                        // 忽略格式錯誤行
                        continue;
                    }
                }
                

                string API = "http://127.0.0.1:4433";
                List<batch_inventory_exportClass> result = batch_inventory_exportClass.add(API, batch_Inventory_ExportClasses, "自動匯入");
                returnData returnData = new returnData();
                returnData.Data = result;
                Logger.LogAddLine($"batch_inventory_export");
                Logger.Log($"batch_inventory_export", $"{returnData.JsonSerializationt(true)}");
                Logger.LogAddLine($"batch_inventory_export");
                //List<batch_inventory_exportClass> batch_Inventory_ExportClasses1 = new List<batch_inventory_exportClass>();
                //batch_Inventory_ExportClasses.Add(new batch_inventory_exportClass
                //{
                //    GUID = "001928bf-568b-4a9a-8a39-c565082005d1"
                //});
                //List<batch_inventory_exportClass> result_1 = batch_inventory_exportClass.update_state_done_by_GUID(API, batch_Inventory_ExportClasses, "測試");

                Console.WriteLine($"共匯入{result.Count}筆");
                //Console.ReadKey(); // 等待按鍵輸入
                System.Threading.Thread.Sleep(60000);


            }
        }
        class DrugRecord
        {
            public string Ward { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
        }
    }
}

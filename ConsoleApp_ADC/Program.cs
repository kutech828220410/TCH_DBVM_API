using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace ConsoleApp_ADC
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Administrator\OneDrive - 鴻森智能科技有限公司\8.部中\HI103946P00.txt"; // 修改為你的檔案路徑
            List<DrugRecord> drugRecords = new List<DrugRecord>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("Big5")))
            {
                string line;
                string currentWard = "";  // 儲存當前護理站名稱

                while ((line = reader.ReadLine()) != null)
                {
                    // 判斷護理站
                    if (line.Contains("護理站："))
                    {
                        var match = Regex.Match(line, @"護理站：(\d+)\s+(.+)");
                        if (match.Success)
                        {
                            string group1 = match.Groups[1].Value;
                            string group2 = match.Groups[2].Value;
                            string group3 = match.Groups[3].Value;

                            currentWard = match.Groups[1].Value;
                        }
                    }

                    // 判斷是否為藥品資料行
                    var drugMatch = Regex.Match(line, @"\s*([A-Z0-9]+)\s+(.+?)\s+(\d+)$");
                    if (drugMatch.Success)
                    {
                        drugRecords.Add(new DrugRecord
                        {
                            Ward = currentWard,
                            Code = drugMatch.Groups[1].Value.Trim(),
                            Name = drugMatch.Groups[2].Value.Trim(),
                            Quantity = int.Parse(drugMatch.Groups[3].Value)
                        });
                    }
                }
                foreach (var record in drugRecords)
                {
                    Console.WriteLine($"{record.Ward} - {record.Code} - {record.Name} - {record.Quantity}");
                }
                //System.Threading.Thread.Sleep(60000);
                Console.WriteLine("請按任意鍵結束...");
                Console.ReadKey(); // 等待按鍵輸入

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

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basic;
using HIS_DB_Lib;
using System.Text;
using System.IO;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DB2VM_API.Controller._API_藥檔取得
{
    [Route("api/[controller]")]
    [ApiController]
    public class BBCM : ControllerBase
    {
        static public string API_Server = "http://127.0.0.1:4433";
        [HttpGet]
        public string Get(string? code)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
            try
            {
                //List<medClass> medClasses = new List<medClass>();
                List<medClass> medClasses = ExcuteEXCELL();
                medClass.add_med_clouds(API_Server, medClasses);
                //string url = @"https://www1.ndmctsgh.edu.tw/pharm/API/MedMainOnline/GetMedSearch?selecttype=EngName&content=Candis";
                //string json_out = Basic.Net.WEBApiGet(url);
                returnData.Code = 200;
                returnData.Result = $"取得藥品資料共{medClasses.Count}筆";
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medClasses;
                return returnData.JsonSerializationt(true);
            }
            catch(Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }       
        }   
        private List<medClass> ExcuteEXCELL()
        {
            List<medClass> medClasses = new List<medClass>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string filePath = @"C:\Users\user\source\repos\MTSC__DBVM_API\WebApi\medPage.xlsx"; ;
            using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("Big5")))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string row = sr.ReadLine();
                    string[] values = row.Split(",");
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Trim('"');
                    }
                    medClass medClass = new medClass
                    {
                        藥品碼 = values[0],
                        藥品名稱 = values[1],
                        包裝單位 = values[3],
                        中西藥 = "西藥"
                    };
                    medClasses.Add(medClass);
                }
            }
            return medClasses;

            
        }
    }
}

using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Basic;
using System.Net;
using HIS_DB_Lib;
using System.Collections.Generic;
using System.Net.Http;

namespace ConsoleApp_medPageCloud
{
    class Program
    {
        static private string API_Server = "http://127.0.0.1:4433";
        static async Task Main(string[] args)
        {
            Logger.Log("藥檔更新開始");
            string url = "http://192.168.8.108:4434/dbvm/bbcm";
            string json = Basic.Net.WEBApiGet(url);
            Logger.Log($"json");
            Logger.Log("藥檔更新結束");
            Logger.Log("藥品圖片更新開始");
            List<medClass> medClasses = medClass.get_med_cloud(API_Server);
            List<medPicClass> picClasses = new List<medPicClass>();
            foreach(var item in medClasses)
            {
                if(item.圖片網址.StringIsEmpty() == false)
                {
                    string base64String = await DownloadPic(item.藥品碼,item.圖片網址);
                    if(base64String.StringIsEmpty() == false)
                    {
                        medPicClass medPicClass = new medPicClass
                        {
                            藥碼 = item.藥品碼,
                            藥名 = item.藥品名稱,
                            副檔名 = "jpg",
                            pic_base64 = base64String
                        };
                        picClasses.Add(medPicClass);
                    }
                    
                }
            }
            medPicClass.add(API_Server, picClasses);
            Logger.Log("藥品圖片更新結束");
        }
        public static async Task<string> DownloadPic(string code,string url)
        {
            string base64String = "";
            using (HttpClient client = new HttpClient())
            {
                try 
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(url);

                    // 將位元組陣列轉換為Base64字串
                    base64String = Convert.ToBase64String(imageBytes);
                }
                catch(Exception ex)
                {
                    //Logger.Log($"發生錯誤: {ex.Message}\ncode:{code}\nurl:{url}");
                    Logger.Log($"code:{code}\nurl:{url}");

                }
            }
            return base64String;
        }
    }
}

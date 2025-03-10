using System;
using Basic;
namespace ConsoleApp_medPageCloud
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("藥檔更新開始");
            string url = "http://192.168.19.200:443/api/BBCM";
            string json = Basic.Net.WEBApiGet(url);
            Console.WriteLine("藥檔更新結束");
            Console.WriteLine("藥品圖片更新開始");
            url = "http://192.168.19.200:443/api/med_pic";
            json = Basic.Net.WEBApiGet(url);
            Console.WriteLine("藥品圖片更新結束");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using SQLUI;
namespace batch_ud_station
{
    class Program
    {
        static string API_Server = "http://127.0.0.1:4433";
        static string[] wardCodes = new string[]
     {
    "2130",  // ＩＣＵ(三)
    "3220",  // 嬰兒室
    "5410",  // 五樓Ａ區護理站
    "5420",  // 九樓Ａ區護理站
    "5430",  // 九樓Ｂ區護理站
    "5450",  // 七樓Ａ區護理站
    "5460",  // 六樓Ｂ區護理站
    "546A",  // 六樓Ａ區護理站
    "546B",  // 新生兒ＩＣＵ
    "546C",  // 病嬰室
    "546D",  // 小兒ＩＣＵ
    "5480",  // 七樓Ｂ區護理站
    "548A",  // 八樓Ａ區護理站
    "548B",  // 八樓Ｂ區護理站
    "5490",  // 十樓Ａ區護理站
    "5491",  // 十樓B區漸凍人護理站
    "549B",  // 十樓B區護理站
    "549C",  // 11A
    "549D",  // 十一樓Ｂ區護理站
    "5520",  // 精神科慢性病房
    "5580",  // H8RCW護理站
    "5600",  // 洗腎室(ＨＤ)
    "5800",  // OPD
    "5801",  // OPD(1)
    "5910",  // ＩＣＵ(一)
    "591A",  // ＩＣＵ(二)
    "591B",  // ＲＣＣ
    "5931",  // (3F)復健病房(長照A3)
    "8415",  // １２樓負壓隔離病房護
    "8894",  // 身心整合病房7H
    "8937",  // (4F)新長照大樓4樓護?
    "8938",  // (7F)新長照大樓7樓護?
    "8939"   // (8F)新長照大樓8樓護?
     };
        static void Main(string[] args)
        {
            for (int i = 0; i < wardCodes.Length; i++)
            {
                Logger.LogAddLine();
                string station = wardCodes[i];
                Logger.Log($"開始取得長期醫令,Station:{station}");
                string url = $"{API_Server}/dbvm/bbar/{station}";

                string json_out =  Basic.Net.WEBApiGet(url);
                Logger.Log($"{json_out}");
                Logger.LogAddLine();
            }
        }
    }
}

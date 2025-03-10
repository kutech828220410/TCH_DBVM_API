using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using HIS_DB_Lib;
using Basic;
using Newtonsoft.Json;
using System.ComponentModel;


namespace DB2VM_API
{
    public enum enum_yc_pha_order
    {
        [Description("DATETIMESEQ,VARCHAR,17,NONE")]
        DATETIMESEQ, // Date time Sequence
        [Description("ID_PATIENT,VARCHAR,8,NONE")]
        ID_PATIENT, // 病歷號
        [Description("NM_PATIENT,VARCHAR,50,NONE")]
        NM_PATIENT, // 病人姓名
        [Description("CD_FROM,VARCHAR,1,NONE")]
        CD_FROM, // 來源:O.門診 E.急診 I.住院
        [Description("DT_REGISTER,VARCHAR,8,NONE")]
        DT_REGISTER, // 看診日(YYYYMMDD)
        [Description("ST_SEQNO,VARCHAR,10,NONE")]
        ST_SEQNO, // 就醫序號 or 住院號
        [Description("ST_BEDNO,VARCHAR,8,DEFAULT NULL")]
        ST_BEDNO, // 病人床號
        [Description("IT_TAKING,INT,11,NONE")]
        IT_TAKING, // 領第幾次(慢簽領第幾次)
        [Description("IT_MODIFY,INT,11,NONE")]
        IT_MODIFY, // 修改第幾次(處方修改)
        [Description("IT_DRUGNO,INT,11,NONE")]
        IT_DRUGNO, // 領藥號
        [Description("CD_ORDER,VARCHAR,1,NONE")]
        CD_ORDER, // 類別:N.門急 B.首日量 S.臨時 M.出院帶藥
        [Description("DM_DRUG,VARCHAR,14,NONE")]
        DM_DRUG, // 開立時間(YYYYMMDDHHMISS)
        [Description("ID_DRUG,VARCHAR,8,NONE")]
        ID_DRUG, // 藥品代碼
        [Description("NM_DRUG,VARCHAR,60,NONE")]
        NM_DRUG, // 藥品名稱
        [Description("CD_DRUG,VARCHAR,1,NONE")]
        CD_DRUG, // 藥品類別:N.一般 1.管一 2.管二 3.管三 4.管四
        [Description("DB_DOSE,FLOAT,NONE,NONE")]
        DB_DOSE, // 藥品單次服用劑量
        [Description("IT_DAYS,INT,11,NONE")]
        IT_DAYS, // 服藥天數
        [Description("DB_AMOUNT,FLOAT,NONE,NONE")]
        DB_AMOUNT, // 藥品總量
        [Description("ST_PATH,VARCHAR,6,NONE")]
        ST_PATH, // 服用方法
        [Description("ST_FREQUENCY,VARCHAR,15,NONE")]
        ST_FREQUENCY, // 服用時間(UD管藥領藥藥籤需要
        [Description("DM_PRCESS,VARCHAR,14,NONE")]
        DM_PRCESS, // 資料處理時間
        [Description("CD_READ,VARCHAR,1,DEFAULT 'N'")]
        CD_READ, // 智慧藥櫃讀取否:N.未讀取 Y.已讀取
        [Description("DM_READ,VARCHAR,15,DEFAULT NULL")]
        DM_READ, // 智慧藥櫃讀取時間
        [Description("CD_CANCEL,VARCHAR,1,DEFAULT 'N'")]
        CD_CANCEL, // 藥品作廢否:N.未作廢 Y.作廢
        [Description("DM_CANCEL,VARCHAR,14,DEFAULT NULL")]
        DM_CANCEL, // 藥品作廢時間
        [Description("ST_HISORDKEY,VARCHAR,50,NONE")]
        ST_HISORDKEY, // HIS醫令關聯值
        [Description("ST_QRCODE,VARCHAR,50,NONE")]
        ST_QRCODE // 藥袋QRCode值
    }
    public class phaOrderClass
    {
        [JsonPropertyName("DATETIMESEQ")]
        public string DATETIMESEQ { get; set; }

        [JsonPropertyName("ID_PATIENT")]
        public string ID_PATIENT { get; set; }

        [JsonPropertyName("NM_PATIENT")]
        public string NM_PATIENT { get; set; }

        [JsonPropertyName("CD_FROM")]
        public string CD_FROM { get; set; } // O.門診 E.急診 I.住院

        [JsonPropertyName("DT_REGISTER")]
        public string DT_REGISTER { get; set; } // YYYYMMDD

        [JsonPropertyName("ST_SEQNO")]
        public string ST_SEQNO { get; set; } // 或住院號

        [JsonPropertyName("ST_BEDNO")]
        public string ST_BEDNO { get; set; }

        [JsonPropertyName("IT_TAKING")]
        public string IT_TAKING { get; set; } // 慢簽領第幾次

        [JsonPropertyName("IT_MODIFY")]
        public string IT_MODIFY { get; set; } // 處方修改

        [JsonPropertyName("IT_DRUGNO")]
        public string IT_DRUGNO { get; set; }

        [JsonPropertyName("CD_ORDER")]
        public string CD_ORDER { get; set; } // N.門急 B.首日量 S.臨時 M.出院帶藥

        [JsonPropertyName("DM_DRUG")]
        public string DM_DRUG { get; set; } // YYYYMMDDHHMISS

        [JsonPropertyName("ID_DRUG")]
        public string ID_DRUG { get; set; }

        [JsonPropertyName("NM_DRUG")]
        public string NM_DRUG { get; set; }

        [JsonPropertyName("CD_DRUG")]
        public string CD_DRUG { get; set; } // N.一般 1.管一 2.管二 3.管三 4.管四

        [JsonPropertyName("DB_DOSE")]
        public string DB_DOSE { get; set; }

        [JsonPropertyName("IT_DAYS")]
        public string IT_DAYS { get; set; }

        [JsonPropertyName("DB_AMOUNT")]
        public string DB_AMOUNT { get; set; }

        [JsonPropertyName("ST_PATH")]
        public string ST_PATH { get; set; }

        [JsonPropertyName("ST_FREQUENCY")]
        public string ST_FREQUENCY { get; set; } // UD管藥領藥藥籤需要

        [JsonPropertyName("DM_PRCESS")]
        public string DM_PRCESS { get; set; }

        [JsonPropertyName("CD_READ")]
        public string CD_READ { get; set; } // N.未讀取 Y.已讀取

        [JsonPropertyName("DM_READ")]
        public string DM_READ { get; set; }

        [JsonPropertyName("CD_CANCEL")]
        public string CD_CANCEL { get; set; } // N.未作廢 Y.作廢

        [JsonPropertyName("DM_CANCEL")]
        public string DM_CANCEL { get; set; }

        [JsonPropertyName("ST_HISORDKEY")]
        public string ST_HISORDKEY { get; set; }

        [JsonPropertyName("ST_QRCODE")]
        public string ST_QRCODE { get; set; }
    }
    public class orderlistClass
    {
        [JsonPropertyName("opdDate")]
        public string 開方日期 { get; set; }
        [JsonPropertyName("patientNo")]
        public int 病歷號 { get; set; }
        [JsonPropertyName("medicationOrderNumber")]
        public int 領藥號 { get; set; }
        [JsonPropertyName("patientName")]
        public string 病人姓名 { get; set; }
        [JsonPropertyName("nsName")]
        public string 病房 { get; set; }
        [JsonPropertyName("bedNo")]
        public string 床號 { get; set; }
        [JsonPropertyName("orderDate")]
        public string orderDate { get; set; }
        [JsonPropertyName("sheetNo")]
        public string 藥袋條碼 { get; set; }
        [JsonPropertyName("medicationItems")]
        public List<MedicationOrder> medicationItems { get; set; }

        static public orderlistClass get_order(string Barcode)
        {
            string url = "";
            if (Barcode.Length > 12)
            {
                url = $"http://192.168.16.230:8132/api/medication/scanclinicmedicationbag?barcode={Barcode}";
            }
            else
            {
                url = $"http://192.168.16.230:8132/api/Medication/ScanInpMedicationBag?BarCode={Barcode}";
            }
            returnData returnData = new returnData();
            string json_out = Net.WEBApiGet(url);
            orderlistClass orderlistClass = json_out.JsonDeserializet<orderlistClass>();
            if(orderlistClass.medicationItems.Count == 0)
            {
                url = $"http://192.168.16.230:8132/api/Medication/ScanDisHomeMedicationBag?BarCode={Barcode}";
                returnData = new returnData();
                json_out = Net.WEBApiGet(url);
                orderlistClass = json_out.JsonDeserializet<orderlistClass>();
            }
            return orderlistClass;
        }
        static public List<orderlistClass> get_order_by_date(string date)
        {
            string url = $"http://192.168.16.230:8132/api/Medication/GetInpControlOrderList?Date={date}";
            returnData returnData = new returnData();
            string json_out = Net.WEBApiGet(url);
            List<orderlistClass> orderlistClasses = json_out.JsonDeserializet<List<orderlistClass>>();
            return orderlistClasses;
        }

    }
    public class MedicationOrder
    {
        [JsonPropertyName("code")]
        public string 料號 { get; set; }
        [JsonPropertyName("fullName")]
        public string 藥品名稱 { get; set; }
        [JsonPropertyName("qty")]
        public double 單次劑量 { get; set; }
        [JsonPropertyName("useName")]
        public string 頻次 { get; set; }
        [JsonPropertyName("medthodName")]
        public string 途徑 { get; set; }
        [JsonPropertyName("tqty")]
        public double 交易量 { get; set; }
        [JsonPropertyName("acntPtr")]
        public int 批序 { get; set; }
        [JsonPropertyName("type")]
        public string 藥袋類型 { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Entity
{
    public class CLog
    {
        static private CLog m_Instance;
        private List<CLogRec> m_LogList;

        private CLog()
        {
            this.m_LogList = new List<CLogRec>();
        }

        public static CLog CreateInstance()
        {
            if (CLog.m_Instance == null)
                CLog.m_Instance = new CLog();
            return CLog.m_Instance;

        }

        public void Append(string category,
            string source_tag,
            string source_idenrifier,
            string path,
            string detail_attribute_name,
            string detail_attribute_old_value,
            string detail_attribute_new_value)
        {
            CLogRec neoLog = new CLogRec();
            neoLog.time_stamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            neoLog.source_tag_name = source_tag;
            neoLog.source_identifier = source_idenrifier;
            neoLog.path = path;
            neoLog.category = category;
            neoLog.detail_attribute_name = detail_attribute_name;
            neoLog.detail_attribute_old_value = detail_attribute_old_value;
            neoLog.detail_attribute_new_value = detail_attribute_new_value;
            m_LogList.Add(neoLog);
        }

        public List<CLogRec> GetLogs()
        {
            return m_LogList;
        }
        public List<CSummaryRec> GetSummary()
        {
            List<CSummaryRec> result = new List<CSummaryRec>();
            ILookup<string, CLogRec> tmpLookup = (from CLogRec tmp in m_LogList
                                                  select tmp).ToLookup(x => x.source_tag_name + "@#@" + x.detail_attribute_name);
            foreach (var tmp in tmpLookup)
            {
                string[] key = System.Text.RegularExpressions.Regex.Split(tmp.Key, "@#@");
                int count = tmp.Count();
                CSummaryRec rec = new CSummaryRec()
                {
                    source_tag_name = key[0],
                    attribute_name = key[1],
                    count = count
                };
                result.Add(rec);
            }
            return result;
        }

        public string FlushAsText()
        {
            string resultText = "";
            foreach (CLogRec logSturct in m_LogList)
            {
                resultText = resultText +
                    "\t" + logSturct.time_stamp + "\t\n" +
                    "\t" + logSturct.category + "\t\n" +
                    "\t" + logSturct.source_tag_name + "\t\n" +
                    "\t" + logSturct.source_identifier + "\t\n" +
                    "\t" + logSturct.path + "\t\n" +
                    "\t" + logSturct.detail_attribute_name +
                    "\t" + logSturct.detail_attribute_old_value + "\t\n" +
                    "\t" + logSturct.detail_attribute_new_value + "\n\n";
            }
            m_LogList.Clear();
            return resultText;
        }

        public void Flush()
        {
            m_LogList.Clear();
        }

    }
}

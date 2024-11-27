using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataTansfer
{
    public class ForteRTDataList 
    {
        private  readonly List<string> ForteHdrList;
        private ClsSQLhandler SqlHandler = ClsSQLhandler.Instance;
        private DataTable HdrTable = new DataTable();
        private readonly List<Tuple<string, string>> SqlList = new List<Tuple<string, string>>();

        public ForteRTDataList()
        {
            HdrTable = SqlHandler.GetSqlScema();
            ForteHdrList = new List<string>();

            foreach (DataRow item in HdrTable.Rows)
            {
                SqlList.Add(new Tuple<string, string>(item[1].ToString(), item[2].ToString()));
                ForteHdrList.Add(item[1].ToString());
            }
        }
        public IEnumerable<string> ForteDataSource => ForteHdrList;



    }

}

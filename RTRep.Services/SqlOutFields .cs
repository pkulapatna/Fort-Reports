using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTRep.Services
{
    public class SqlOutFields : BindableBase
    {
        private string _clientfieldName;
        public string ClientFieldName
        {
            get { return _clientfieldName; }
            set { SetProperty(ref _clientfieldName, value); }
        }
        private string _clientfieldType;
        public string ClientFieldType
        {
            get { return _clientfieldType; }
            set { SetProperty(ref _clientfieldType, value); }
        }

        private string _fortefieldName;
        public string ForteFieldName
        {
            get { return _fortefieldName; }
            set { SetProperty(ref _fortefieldName, value); }
        }
        private string _fortefieldType;
        public string ForteFieldType
        {
            get { return _fortefieldType; }
            set { SetProperty(ref _fortefieldType, value); }
        }

        public SqlOutFields(string clientName, string clientType, string forteName, string forteType)
        {
            this.ClientFieldName = clientName;
            this.ClientFieldType = clientType;
            this.ForteFieldName = forteName;
            this.ForteFieldType = forteType;
        }

    }
}

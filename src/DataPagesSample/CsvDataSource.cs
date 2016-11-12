using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Pages;

namespace DataPagesSample
{
    public class CsvDataSource : BaseDataSource
    {
        public string Path { get; set; }

        protected override Task<IList<IDataItem>> GetRawData()
        {
            throw new NotImplementedException();
        }

        protected override object GetValue(string key)
        {
            throw new NotImplementedException();
        }

        protected override bool SetValue(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}

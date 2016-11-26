using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Pages;
using FSharp.Data;
using Microsoft.FSharp.Core;

namespace DataPagesSample
{
    public class CsvDataRow : BaseDataSource
    {
        private CsvRow raw;
        private CsvFile parent;

        public CsvDataRow(CsvRow raw, CsvFile parent)
        {
            this.raw = raw;
            this.parent = parent;
        }

        protected override Task<IList<IDataItem>> GetRawData()
        {
            IList<IDataItem> data = raw.Columns.Select((c, i) => new DataItem(parent.Headers.Value[i], c)).Cast<IDataItem>().ToList();
            return Task.FromResult(data);
        }

        protected override object GetValue(string key)
        {
            return raw.GetColumn(key);
        }

        protected override bool SetValue(string key, object value)
        {
            throw new NotImplementedException();
        }
    }

    public class CsvDataSource : BaseDataSource
    {
        private readonly ObservableCollection<IDataItem> dataItems;
        private bool initialized;
        
        public CsvDataSource()
        {
            dataItems = new ObservableCollection<IDataItem>();
        }
        
        public string Path { get; set; }
        public string Separator { get; set; } = ",";
        public char Quote { get; set; } = '"';
        public bool HasHeaders { get; set; } = true;

        protected override async Task<IList<IDataItem>> GetRawData()
        {
            if (initialized) return dataItems;

            await Task.Run(() =>
            {
                IsLoading = true;

                var csv = CsvFile.Load(
                            Path,
                            FSharpOption<string>.Some(Separator),
                            FSharpOption<char>.Some(Quote),
                            FSharpOption<bool>.Some(HasHeaders),
                            FSharpOption<bool>.Some(false),
                            FSharpOption<int>.Some(0));

                var rows = csv.Rows;

                foreach (var item in rows.Select(r => new CsvDataRow(r, csv)).Select((r, i) => new DataItem(i.ToString(), r)))
                {
                    dataItems.Add(item);
                }

                IsLoading = false;
            });

            initialized = true;

            return dataItems;
        }

        protected override object GetValue(string key)
        {
            return dataItems.FirstOrDefault(d => d.Name == key)?.Value;
        }

        protected override bool SetValue(string key, object value)
        {
            var target = dataItems.FirstOrDefault(d => d.Name == key);

			if (target == null)
			{
				dataItems.Add(new DataItem(key, value));
				return true;
			}

			if (target.Value == value)
            {
				return false;
            }

			target.Value = value;

			return true;
        }
    }
}

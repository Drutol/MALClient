using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Foundation;
using MALClient.Adapters;
using Newtonsoft.Json;

namespace MALClient.iOS.Adapters
{
	public class ApplicationDataService : IApplicationDataService
	{
		string _fileName = "ApplicationDataDictionary";
		public object this[string key]
		{
			get
			{
				var x = GetApplicationDataDictionary();
				if (x.ContainsKey(key))
				{
					switch (x[key].Item2)
					{
						case TypeCode.String:
						case TypeCode.Boolean:
							return x[key].Item1;
						case TypeCode.Int32:
							return Convert.ChangeType(x[key].Item1, TypeCode.Int32);
						case TypeCode.Int64:
							return Convert.ChangeType(x[key].Item1, TypeCode.Int64);
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else 
					return default(object);
				//To się pisze ... samo ... @ 17.09.2016 Ilona zobaczyła jak coś robie
			}
			set
			{
				var dictionary = GetApplicationDataDictionary();
				dictionary.Remove(key);
				dictionary.Add(key, Tuple.Create(value,Type.GetTypeCode(value.GetType())));
				SaveApplicationDataDictionary(dictionary);
			}
		}

		Dictionary<string, Tuple<object,TypeCode>> GetApplicationDataDictionary()
		{
			var originFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var filepath = Path.Combine(originFolder, _fileName);
			if (!File.Exists(filepath))
			{
				File.Create(filepath).Dispose();
			}
			var data = File.ReadAllText(filepath);

			return JsonConvert.DeserializeObject<Dictionary<string, Tuple<object, TypeCode>>>(data) ?? new Dictionary<string, Tuple<object, TypeCode>>();
		}

		void SaveApplicationDataDictionary(Dictionary<string, Tuple<object,TypeCode>> applicationDataDictionary)
		{
			var targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var file = Path.Combine(targetFolder, _fileName);

			var json = JsonConvert.SerializeObject(applicationDataDictionary);

			File.WriteAllText(file, json);
		}
	}
}

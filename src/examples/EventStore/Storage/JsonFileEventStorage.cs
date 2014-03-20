namespace EventStore.Storage
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	using Model;

	using Newtonsoft.Json;

	public class JsonFileEventStorage : IEventStorage
	{
		readonly DirectoryInfo _folder;

		public JsonFileEventStorage(string folderName)
		{
			_folder = Directory.CreateDirectory(folderName);

			ClearStorageFolder(_folder);
		}

		static void ClearStorageFolder(DirectoryInfo folder)
		{
			folder.Delete(true);
			folder.Create();
		}

		public IEnumerable<IDomainEvent> Load(Type type, Guid id)
		{
			var path = GetFilePathFor(type.Name, id);
			if (!File.Exists(path))
			{
				return new List<IDomainEvent>();
			}

			var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
			return (IEnumerable<IDomainEvent>)JsonConvert.DeserializeObject(File.ReadAllText(path, Encoding.UTF8), typeof(IEnumerable<IDomainEvent>), settings);
		}

		public void Save(Type type, Guid id, IEnumerable<IDomainEvent> events)
		{
			var path = GetFilePathFor(type.Name, id);

			var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
			File.WriteAllText(path, JsonConvert.SerializeObject(events, settings));
		}

		private string GetFilePathFor(string type, Guid id)
		{
			return Path.Combine(_folder.FullName, string.Format("{0}-{1}.json", type, id));
		}
	}
}
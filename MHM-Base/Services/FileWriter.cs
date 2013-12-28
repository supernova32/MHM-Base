using System.IO;

namespace MHMBase
{
	public class FileWriter {
		readonly string _path;

		public FileWriter (string path)	{
			_path = path;
		}

		public void SaveXML (string file, string name) {
			string filePath = Path.Combine (_path, name);
			File.WriteAllText(filePath, file);
		}

	}
}


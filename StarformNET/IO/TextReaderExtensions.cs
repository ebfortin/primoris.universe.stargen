using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Primoris.Universe.Stargen.IO
{
	public static class TextReaderLineExtensions
	{
		public static IEnumerable<string> ReadLines(this string path)
		{
			return ReadLines(File.OpenText(path));
		}

		public static IEnumerable<string> ReadLines(this Stream str)
		{
			return ReadLines(new StreamReader(str));
		}

		public static IEnumerable<string> ReadLines(this TextReader reader)
		{
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				yield return line;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Primoris.Universe.Stargen.IO
{
	public static class TextReaderLineExtensions
	{
		/// <summary>
		/// Reads the lines.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>IEnumerable of lines given as strings.</returns>
		public static IEnumerable<string> ReadLines(this string path)
		{
			return ReadLines(File.OpenText(path));
		}

		/// <summary>
		/// Reads the lines.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>IEnumerable of lines given as strings.</returns>
		public static IEnumerable<string> ReadLines(this Stream str)
		{
			return ReadLines(new StreamReader(str));
		}

		/// <summary>
		/// Reads the lines.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns>IEnumerable of lines given as strings.</returns>
		public static IEnumerable<string> ReadLines(this TextReader reader)
		{
			string? line;
			while ((line = reader.ReadLine()) != null)
			{
				yield return line;
			}
		}
	}
}

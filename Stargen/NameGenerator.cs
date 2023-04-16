using System;
using System.Reflection;
using System.Collections.Generic;
using Primoris.Universe.Stargen.IO;
using System.IO;

namespace Primoris.Universe.Stargen
{

	/// <summary>
	/// Generates random names based on the statistical weight of letter sequences
	/// in a collection of sample names
	/// </summary>
	/// <remarks>
	/// From http://www.siliconcommandergames.com/MarkovNameGenerator.htm
	/// </remarks>
	public class NameGenerator
	{
		public NameGenerator(int order = 3, int minLength = 6) 
			: this(Assembly.GetExecutingAssembly().GetManifestResourceStream("Primoris.Universe.Stargen.Resources.starnames.csv")!.ReadLines(),
				  Assembly.GetExecutingAssembly().GetManifestResourceStream("Primoris.Universe.Stargen.Resources.greekletters.csv")!.ReadLines(),
				  order, 
				  minLength)
		{
		}

		//constructor
		public NameGenerator(IEnumerable<string> sampleNames, IEnumerable<string> prefixes, int order = 3, int minLength = 6)
		{
			_prefixes = new List<string>(prefixes);

			//fix parameter values
			if (order < 1)
				order = 1;
			if (minLength < 1)
				minLength = 1;

			_order = order;
			_minLength = minLength;

			//split comma delimited lines
			foreach (string line in sampleNames)
			{
				string[] tokens = line.Split(',');
				foreach (string word in tokens)
				{
					string upper = word.Trim().ToUpper();
					if (upper.Length < order + 1)
						continue;
					_samples.Add(upper);
				}
			}

			//Build chains            
			foreach (string word in _samples)
			{
				for (int letter = 0; letter < word.Length - order; letter++)
				{
					string token = word.Substring(letter, order);
					List<char> entry;
					if (_chains.ContainsKey(token))
						entry = _chains[token];
					else
					{
						entry = new List<char>();
						_chains[token] = entry;
					}
					entry.Add(word[letter + order]);
				}
			}
		}

		//Get the next random name
		public string NextName(bool usePrefix = true, bool useSuffix = false)
		{
			int rndMinLength = _rnd.Next(_minLength, 2 * _minLength);

			//get a random token somewhere in middle of sample word                
			string s = "";
			do
			{
				int n = _rnd.Next(_samples.Count);
				int nameLength = _samples[n].Length;
				s = _samples[n].Substring(_rnd.Next(0, _samples[n].Length - _order), _order);
				while (s.Length < nameLength)
				{
					string token = s.Substring(s.Length - _order, _order);
					char c = GetLetter(token);
					if (c != '?')
						s += GetLetter(token);
					else
						break;
				}

				if (s.Contains(" "))
				{
					string[] tokens = s.Split(' ');
					s = "";
					for (int t = 0; t < tokens.Length; t++)
					{
						if (tokens[t] == "")
							continue;
						if (tokens[t].Length == 1)
							tokens[t] = tokens[t].ToUpper();
						else
							tokens[t] = tokens[t].Substring(0, 1) + tokens[t].Substring(1).ToLower();
						if (s != "")
							s += " ";
						s += tokens[t];
					}
				}
				else
					s = s.Substring(0, 1) + s.Substring(1).ToLower();
			}
			while (/*_used.Contains(s) ||*/ s.Length < rndMinLength);

			s = (!usePrefix ? "" : _prefixes[_rnd.Next(_prefixes.Count - 1)] + " ") + s;
			s = s + (!useSuffix ? "" : " " + _suffixes[_rnd.Next(_suffixes.Count - 1)]);
			s = s.Trim();

			//_used.Add(s);
			return s;
		}

		//Reset the used names
		public void Reset()
		{
			_used.Clear();
		}

		//private members
		private Dictionary<string, List<char>> _chains = new Dictionary<string, List<char>>();
		private List<string> _samples = new List<string>();
		private List<string> _prefixes;
		private List<string> _suffixes = new List<string>() { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV" };
		private List<string> _used = new List<string>();
		private Random _rnd = new Random();
		private int _order;
		private int _minLength;

		//Get a random letter from the chain
		private char GetLetter(string token)
		{
			if (!_chains.ContainsKey(token))
				return '?';
			List<char> letters = _chains[token];
			int n = _rnd.Next(letters.Count);
			return letters[n];
		}
	}
}

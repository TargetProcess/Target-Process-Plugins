namespace Tp.Search.Model.Query
{
	class ParsedQuery
	{
		private readonly string _words;
		private readonly string _numbers;

		public ParsedQuery(string words = "", string numbers = "")
		{
			_words = words;
			_numbers = numbers;
		}

		public string Words
		{
			get { return _words; }
		}

		public string Numbers
		{
			get { return _numbers; }
		}

		public string Full
		{
			get
			{
				if (!string.IsNullOrEmpty(Words) && string.IsNullOrEmpty(Numbers))
			{
				return Words;
			}
			if (!string.IsNullOrEmpty(Numbers) && string.IsNullOrEmpty(Words))
			{
				return Numbers;
			}
			return Words + " " + Numbers;
			}
		}
	}
}
namespace Quiz
{
    public class QuizzesHistory: LINQFileWorker
    {
        public string NamePlayer { get; set; }
        public DateTime dateOfStart;
        public DateTime dateOfEnd;
        public int Score { get; set; }
        public string NameTheme { get; set; }

        public QuizzesHistory(string Nametheme, int score, DateTime dateOfStart, DateTime dateOfEnd)
        {
            this.dateOfStart = dateOfStart;
            this.dateOfEnd = dateOfEnd;
            Score = score;
            NameTheme = Nametheme;
        }

        public QuizzesHistory() : this("", 0, DateTime.Now, DateTime.Now) { }
    }
}

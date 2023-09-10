using System.Xml.Linq;

namespace Quiz
{
    public class Player: LINQFileWorker
    {
        private static string path = "PlayersAccounts.xml";
        private string login;
        private string password;
        private DateOnly dateOfBirth;
        private List<QuizzesHistory> quizzesHistory=new List<QuizzesHistory>();

        public Player(string login, string password, DateOnly dateOfBirth)
        {
            this.login = login;
            this.password = password;
            this.dateOfBirth = dateOfBirth;

            if (!File.Exists(path)) SaveList(null);
        }
        public Player() : this("", "", new DateOnly()) { }

        public static void FileToListHistoryOfAllQuizzes(ref List<QuizzesHistory> historyOfAllQuizzes)
        {
            var players = LINQFileWorker.ReadFile(path, "Player");
            if (players != null)
            {
                foreach (var player in players)
                {
                    List<QuizzesHistory> quizzesHistory = new List<QuizzesHistory>();
                    foreach (var item in player.Element("QuizzesHistory").Elements("QuizHistory"))
                    {
                        QuizzesHistory history = new QuizzesHistory();
                        history.NamePlayer = player.Attribute("Login").Value;
                        history.NameTheme = item.Element("ThemeName").Value;
                        history.Score = Convert.ToInt32(item.Element("Score").Value);
                        history.dateOfStart = Player.InFileToDate("DateOfStart", item);
                        history.dateOfEnd = Player.InFileToDate("DateOfEnd", item);
                        historyOfAllQuizzes.Add(history);
                    }
                }
            }
        }

        public static void AddXElement(Player player)
        {
            var document = XDocument.Load(path);
            document.Element("Players").Add(CreateXElement(player));
            document.Save(path);
        }

        public static void AddXElements(List<Player> players)
        {
            var document = XDocument.Load(path);
            foreach (var player in players)
            {
                document.Element("Players").Add(CreateXElement(player));
            }
            document.Save(path);
        }

        public string GetLogin()
        {
            return login;
        }

        public bool LogIn()
        {
            Console.Clear();

            while (true)
            {
                XElement player = null;
                Console.Write("Enter your login:\n=>");
                string loginBuf = Console.ReadLine().ToLower();

                if (!SearchLogin(loginBuf, ref player))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid login!\n");
                    continue;
                }

                Console.Write("Enter your password:\n=>");
                string passwordBuf = Console.ReadLine();

                if (passwordBuf != player.Element("Password").Value)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid password!\n");
                    continue;
                }
                
                login = player.Attribute("Login").Value;
                password = player.Element("Password").Value;
                dateOfBirth = new DateOnly(Convert.ToDateTime(player.Element("DateOfBirth").Value).Year,
                    Convert.ToDateTime(player.Element("DateOfBirth").Value).Month,
                    Convert.ToDateTime(player.Element("DateOfBirth").Value).Day);

                foreach (var item in player.Element("QuizzesHistory").Elements("QuizHistory"))
                {
                    QuizzesHistory history = new QuizzesHistory();
                    history.NameTheme = item.Element("ThemeName").Value;
                    history.Score = Convert.ToInt32(item.Element("Score").Value);
                    history.dateOfStart = InFileToDate("DateOfStart", item);
                    history.dateOfEnd = InFileToDate("DateOfEnd", item);
                    quizzesHistory.Add(history);
                }
                return true;
            }
        }

        public bool Register()
        {
            Console.Clear();
            while (true)
            {
                Console.Write("Enter login:\n=>");
                string l = Console.ReadLine().ToLower();
                if (!ChangeLogin(l)) continue;

                Console.Write("Enter password: \n=>");
                string p = Console.ReadLine();
                if (!ChangePassword(p)) continue;

                Console.Write("Enter the year of birth:\n=>");
                string y = Console.ReadLine();
                Console.Write("Enter the month of birth:\n=>");
                string m = Console.ReadLine();
                Console.Write("Enter the day of birth:\n=>");
                string d = Console.ReadLine();

                if (!ChangeDateOfBirth(y, m, d)) continue;
                return true;
            }
        }

        public void AddQuizToPersonalHistory(string historyName, int point, DateTime dateOfStart, DateTime dateOfEnd)
        {
            quizzesHistory.Add(new QuizzesHistory(historyName, point, dateOfStart, dateOfEnd));
        }

        public void AddUpdateToFile()
        {
            IEnumerable<XElement> players = DeleteXElement(login, "Login", path, "Player");

            var objectLINQ = new XElement("Players");
            objectLINQ.Add(players);
            objectLINQ.Add(CreateXElement(this));

            var document = new XDocument();
            document.Add(objectLINQ);
            document.Save(path);
        }

        public static DateTime InFileToDate(string nameElement, XElement xElement)
        {
            int day = Convert.ToInt32(xElement.Element(nameElement).Value.Split("\t")[0].Split(".")[0]);
            int month = Convert.ToInt32(xElement.Element(nameElement).Value.Split("\t")[0].Split(".")[1]);
            int year = Convert.ToInt32(xElement.Element(nameElement).Value.Split("\t")[0].Split(".")[2]);
            int hour = Convert.ToInt32(xElement.Element(nameElement).Value.Split("\t")[1].Split(":")[0]);
            int minute = Convert.ToInt32(xElement.Element(nameElement).Value.Split("\t")[1].Split(":")[1]);
            int seconds = Convert.ToInt32(xElement.Element(nameElement).Value.Split("\t")[1].Split(":")[2]);

            DateTime date = new DateTime(year, month, day, hour, minute, seconds);
            return date;
        }
        public void ShowHistory()
        {
            Console.Clear();
            for (int i=quizzesHistory.Count-1;i>-1;i--)
            {
                Console.WriteLine($"Quiz theme: {quizzesHistory[i].NameTheme}\t Score: {quizzesHistory[i].Score}\t Date of start: {quizzesHistory[i].dateOfStart}\t Date of end: {quizzesHistory[i].dateOfEnd}");
            }
        }

        public bool IsCorrectPassword(string p)
        {
            if (p.Count(x => x == ' ') != 0)
            {
                Console.Clear();
                Console.WriteLine("Invalid characters in the password!");
                return false;
            }
            if (p.Count() < 3)
            {
                Console.Clear();
                Console.WriteLine("Insufficient number of characters!");
                return false;
            }
            return true;
        }

        public bool IsCorrectDateOfBirth(string y,string m, string d, ref int year,ref int month,ref int day)
        {
            if (!int.TryParse(y, out year))
            {
                Console.Clear();
                Console.WriteLine("Invalid year!");
                return false;
            }
            if (year < 0 || year > DateTime.Now.Year)
            {
                Console.Clear();
                Console.WriteLine("Invalid year!");
                return false;
            }

            if (!int.TryParse(m, out month))
            {
                Console.Clear();
                Console.WriteLine("Invalid month!");
                return false;
            }
            if (month < 0 || month > 12)
            {
                Console.Clear();
                Console.WriteLine("Invalid month!");
                return false;
            }

            if (!int.TryParse(d, out day))
            {
                Console.Clear();
                Console.WriteLine("Invalid day!");
                return false;
            }
            if (day < 0 || day > DateTime.DaysInMonth(year, month))
            {
                Console.Clear();
                Console.WriteLine("Invalid day!");
                return false;
            }
            return true;
        }

        public bool ChangePassword(string p)
        {
            if (!IsCorrectPassword(p)) return false;
            password = p;
            return true;
        }

        public bool ChangeDateOfBirth(string y,string m,string d)
        {
            int year=0, month=0, day=0;

            if (!IsCorrectDateOfBirth(y,m,d, ref year,ref month,ref day)) return false;

            DateOnly dateOfBirthBuf = new DateOnly(year, month, day);
            dateOfBirth = dateOfBirthBuf;
            return true;
        }

        private static XElement CreateXElementQuizzesHistory(Player player)
        {
            var elementLINQ = new XElement("QuizzesHistory");
            foreach (var item in player.quizzesHistory)
            {
                var historyLINQ = new XElement("QuizHistory");
                historyLINQ.Add(new XElement("ThemeName", item.NameTheme));
                historyLINQ.Add(new XElement("Score", item.Score));
                historyLINQ.Add(new XElement("DateOfStart",item.dateOfStart.Day+"."+ item.dateOfStart.Month+"."+ item.dateOfStart.Year+"\t"+ item.dateOfStart.Hour+ ":" + item.dateOfStart.Minute + ":" + item.dateOfStart.Second));
                historyLINQ.Add(new XElement("DateOfEnd", item.dateOfEnd.Day + "." + item.dateOfEnd.Month + "." + item.dateOfEnd.Year + "\t" + item.dateOfEnd.Hour + ":" + item.dateOfEnd.Minute+ ":" + item.dateOfEnd.Second));
                elementLINQ.Add(historyLINQ);
            }
            return elementLINQ;
        }

        private static XElement CreateXElement(Player player)
        {
            var playerLINQ = new XElement("Player");
            playerLINQ.Add(new XAttribute("Login", player.login));
            playerLINQ.Add(new XElement("Password", player.password));
            playerLINQ.Add(new XElement("DateOfBirth", player.dateOfBirth));
            playerLINQ.Add(CreateXElementQuizzesHistory(player));
            return playerLINQ;
        }

        private bool IsCorrectLogin(string l)
        {
            XElement player = new XElement("Player");
            if (SearchLogin(l, ref player)) return false;
            if (l.Count(x => x == ' ') != 0)
            {
                Console.Clear();
                Console.WriteLine("Invalid characters in the login!");
                return false;
            }
            if (l.Count() < 3)
            {
                Console.Clear();
                Console.WriteLine("Insufficient number of characters!");
                return false;
            }
            return true;
        }

        private bool ChangeLogin(string l)
        {
            if (!IsCorrectLogin(l)) return false;
            login = l;
            return true;
        }

        private void SaveList(List<Player> objects)
        {
            var objectLINQ = new XElement("Players");

            if (objects != null)
            {
                foreach (var item in objects)
                {
                    objectLINQ.Add(CreateXElement(item));
                }
            }

            var document = new XDocument();
            document.Add(objectLINQ);
            document.Save(path);
        }

        private bool SearchLogin(string loginBuf, ref XElement element)
        {
            loginBuf = loginBuf.ToLower();
            var players = ReadFile(path, "Player");
            var filtered = players.Where(x => x.Attribute("Login").Value == loginBuf);

            if (filtered.Count() == 0) return false;
            else
            {
                element = filtered.First();
                return true;
            }
        }
    }
}


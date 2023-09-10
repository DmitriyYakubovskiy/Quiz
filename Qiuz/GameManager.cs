namespace Quiz
{
    public class GameManager
    {
        private static string path = "PlayersAccounts.xml";
        private Player player=new Player();
        private List<Quiz> quizzes=new List<Quiz>();
        private List<QuizzesHistory> historyOfAllQuizzes=new List<QuizzesHistory>();

        public void PrintQuizzes()
        {
            foreach(var quiz in quizzes)
            {
                Console.WriteLine($"Quiz theme: {quiz.NameTheme}\t Description: {quiz.Description}");
            }
        }

        public void Start()
        {
            quizzes = Quiz.FileToList();

            bool check = false;
            string choice;

            while (!check)
            {
                Console.Clear();
                Console.Write($"0-LogIn\n1-Register\n=>");
                choice = Console.ReadLine();

                if (choice == "0")
                {
                    check = player.LogIn();
                }
                else if (choice == "1")
                {
                    check = player.Register();
                    if (check)
                    {
                        Player.AddXElement(player);
                    }
                }
            }
            while (true)
            {
                Console.Clear();
                Console.WriteLine("0-Exit\n1-Redactor\n2-Game");
                choice = Console.ReadLine();
                if (choice == "0") return;
                if (choice == "1") Redactor();
                if (choice == "2") Game();
            }
        }

        private void Redactor()
        {
            while (true)
            {
                Console.Clear();
                string choise;
                Console.Write("0-Exit\n1-Add quiz\n2-Delete quiz\n3-Change quiz\n=>");
                choise = Console.ReadLine();

                if (choise == "0")
                {
                    Quiz.AddUpdateToFile(quizzes);
                    break;
                }
                if (choise == "1") AddQuiz();
                if(choise == "2") DeleteQuiz();
                if (choise == "3") ChangeQuiz();
            }
        }

        private void AddQuiz()
        {
            Console.Clear();
            while (true)
            {
                Quiz quiz = new Quiz();

                Console.Write("Quiz theme: \n=>");
                string theme = Console.ReadLine();
                
                if(Quiz.SearchQuiz(theme, quizzes,ref quiz))
                {
                    Console.Clear();
                    Console.WriteLine("A quiz with that name already exists!");
                    continue;
                }
                
                Console.Write("Description: \n=>");
                string description = Console.ReadLine();

                Console.Write("Number of questions: \n=>");

                int countOfQuestions;
                string countOfQuestionsString = Console.ReadLine();
                
                if (!int.TryParse(countOfQuestionsString, out countOfQuestions))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid count!");
                    continue;
                }
                if(countOfQuestions < 0)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid count!");
                    continue;
                }
               
                quiz.NameTheme = theme;
                quiz.Description = description;

                for (int i = 0; i < countOfQuestions; i++)
                {
                    Console.Write("Question:\n=>");
                    string question = Console.ReadLine();
                    
                    if (quiz.CheckQuestionInList(question))
                    {
                        Console.Clear();
                        Console.WriteLine("A question with that name already exists!");
                        i--;
                        continue;
                    }

                    Console.Write("Number of answers: \n=>");
                    int k;
                    string str = Console.ReadLine();

                    if (!int.TryParse(str, out k))
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid count!");
                        continue;
                    }
                    if (k < 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid count!");
                        continue;
                    }

                    List<string> listAnswers = new List<string>();

                    for (int j = 0; j < k; j++)
                    {
                        Console.Write($"Answer {j+1}:\n=>");
                        string answer = Console.ReadLine();
                        listAnswers.Add(answer);
                    }
                    quiz.AddQuestion(question, listAnswers);
                }

                quizzes.Add(quiz);
                Quiz.AddXElement(quiz);
                break;
            }
        }

        private void DeleteQuiz()
        {
            Console.Clear();
            while (true)
            {
                Quiz quiz = new Quiz();

                PrintQuizzes();
                Console.WriteLine();
                Console.Write("Quiz theme: \n=>");
                string theme = Console.ReadLine();

                if (!Quiz.SearchQuiz(theme, quizzes, ref quiz))
                {
                    Console.Clear();
                    Console.WriteLine("No quiz found!");
                    continue;
                }
                if(theme.ToLower() == "Mixed quiz".ToLower())
                {
                    Console.Clear();
                    Console.WriteLine("Error!");
                    continue;
                }

                quizzes.Remove(quiz);
                Console.WriteLine("Quiz removed!");
                PressKey();
                break;
            }
        }

        private void ChangeQuiz()
        {
            Console.Clear();
            while (true)
            {
                Quiz quiz = new Quiz();
                PrintQuizzes();
                Console.WriteLine();
                Console.Write("Quiz theme: \n=>");
                string theme = Console.ReadLine();
                string choise;

                if (!Quiz.SearchQuiz(theme, quizzes, ref quiz))
                {
                    Console.Clear();
                    Console.WriteLine("No quiz found!");
                    continue;
                }
                if (theme.ToLower() == "Mixed quiz".ToLower())
                {
                    Console.Clear();
                    Console.WriteLine("Error!");
                    continue;
                }

                Console.Write("0-Exit\n1-Change description\n2-Add question\n3-Delete question\n=>");
                choise = Console.ReadLine();

                int index = quizzes.IndexOf(quiz);

                if (choise == "0") break;
                if (choise == "1")
                {
                    Console.Write("Description: \n=>");
                    string description = Console.ReadLine();

                    quizzes[index].Description = description;
                }
                if (choise == "2")
                {
                    Console.Write("Question:\n=>");
                    string question = Console.ReadLine();

                    if (quiz.CheckQuestionInList(question))
                    {
                        Console.Clear();
                        Console.WriteLine("A question with that name already exists!");
                        continue;
                    }

                    Console.Write("Number of answers: \n=>");
                    int k;
                    string str = Console.ReadLine();

                    if (!int.TryParse(str, out k))
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid count!");
                        continue;
                    }
                    if (k < 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid count!");
                        continue;
                    }

                    List<string> listAnswers = new List<string>();

                    for (int j = 0; j < k; j++)
                    {
                        Console.Write($"Answer {j + 1}:\n=>");
                        string answer = Console.ReadLine();
                        listAnswers.Add(answer);
                    }
                    quizzes[index].AddQuestion(question, listAnswers);
                }
                if (choise == "3")
                {
                    Console.Write("Question:\n=>");
                    string question = Console.ReadLine();

                    if (!quiz.CheckQuestionInList(question))
                    {
                        Console.Clear();
                        Console.WriteLine("Question not found!");
                        continue;
                    }
                    quizzes[index].RemoveQuestion(question);
                }

                Console.WriteLine("Successfully");
                PressKey();
                break;
            }
        }

        private void Game()
        {
            Player.FileToListHistoryOfAllQuizzes(ref historyOfAllQuizzes);
            while (true)
            {
                Console.Clear();
                Console.Write("0-Exit\n1-Start Quiz\n2-Show the results of past quizzes\n3-View top quizzes\n4-Settings\n=>");
                string choice;
                choice = Console.ReadLine();

                if (choice == "0")
                {
                    Quiz.AddUpdateToFile(quizzes);
                    player.AddUpdateToFile();
                    break;
                }
                if (choice == "1") StartQuiz();
                if (choice == "2")
                {
                    player.ShowHistory(); PressKey();
                }
                if (choice == "3") PrintTop();
                if (choice == "4") Settings();
            }
        }

        private void StartQuiz()
        {
            Console.Clear();
            while (true)
            {
                PrintQuizzes();
                Console.WriteLine();
                Console.Write("Quiz theme: \n=>");
                
                string theme = Console.ReadLine();
                Quiz quiz = new Quiz();

                if (!Quiz.SearchQuiz(theme, quizzes, ref quiz))
                {
                    Console.Clear();
                    Console.WriteLine("Quiz not found!");
                    continue;
                }

                int point = 0;

                List<string> lastQuestions = new List<string>();
                DateTime timeStart = DateTime.Now;

                Console.WriteLine($"Quiz theme: {quiz.NameTheme}\t Description: {quiz.Description}");

                while (true)
                {
                    Console.WriteLine();

                    string question;
                    question = quiz.NameTheme == "Mixed quiz" ? quiz.GetQuestionRandomTheme(lastQuestions,quizzes) : quiz.GetQuestion(lastQuestions);
                    lastQuestions.Add(question);

                    if (question == null)
                    {
                        DateTime timeEnd = DateTime.Now;
                        Console.WriteLine("The quiz is over!\t Number of correct answers: "+point);
                        player.AddQuizToPersonalHistory(quiz.NameTheme, point, timeStart, timeEnd);
                        AddQuizToHistory(player.GetLogin(), quiz.NameTheme, point, timeStart, timeEnd);
                        PressKey();
                        break;
                    }

                    Console.WriteLine(question);
                    Console.Write("Enter the answer:\n=>");
                    string answer = Console.ReadLine();
                    
                    if (quiz.NameTheme == "Mixed quiz" ? Quiz.CheckAnswer(question, answer,quizzes) : quiz.CheckAnswer(question, answer))
                    {
                        Console.WriteLine("Correct answer!");
                        point++;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect answer!");
                    }
                }
                break;
            }
        }

        private void PrintTop()
        {
            Console.Clear();
            while (true)
            {
                PrintQuizzes();
                Console.WriteLine();
                Console.Write("Quiz theme: \n=>");

                string theme = Console.ReadLine();
                Quiz quiz = new Quiz();

                if (!Quiz.SearchQuiz(theme, quizzes, ref quiz))
                {
                    Console.Clear();
                    Console.WriteLine("Quiz not found!");
                    continue;
                }

                SortTop();
                int cnt = 0;
                foreach (var item in historyOfAllQuizzes)
                {
                    if (cnt == 20) return;
                    if (item.NameTheme.ToLower() == quiz.NameTheme.ToLower())
                    {
                        Console.WriteLine($"Player: {item.NamePlayer}\t Quiz theme: {item.NameTheme}\t Score: {item.Score}\t Date of start: {item.dateOfStart}\t Date of end: {item.dateOfEnd}");
                        cnt++;
                    }

                }
                PressKey();
                break;
            }
        }

        private void SortTop()
        {
            for (int i = 0; i < historyOfAllQuizzes.Count; i++)
            {
                for (int j = 0; j < historyOfAllQuizzes.Count - i - 1; j++)
                {
                    if (historyOfAllQuizzes[j].Score < historyOfAllQuizzes[j + 1].Score)
                    {
                        QuizzesHistory buf = historyOfAllQuizzes[j];
                        historyOfAllQuizzes[j] = historyOfAllQuizzes[j + 1];
                        historyOfAllQuizzes[j + 1] = buf;
                    }
                }
            }
        }

        public void Settings()
        {
            Console.Clear();
            while (true)
            {
                Console.Write("0-Back\n1-Сhange password\n2-Сhange the date of birth\n=>");
                string choice;
                bool check = false;
                choice = Console.ReadLine();
                Console.Clear();
                if (choice == "0") break;
                else
                {
                    while (!check)
                    {

                        if (choice == "1")
                        {

                            Console.Write("Enter new password: \n=>");
                            string passwordBuf = Console.ReadLine();
                            check = player.ChangePassword(passwordBuf);
                            if (check)
                            {
                                Console.WriteLine("Settings updated!");
                                PressKey();
                            }
                        }
                        else if (choice == "2")
                        {

                            Console.Write("Enter the year of birth:\n=>");
                            string year = Console.ReadLine();

                            Console.Write("Enter the month of birth:\n=>");
                            string month = Console.ReadLine();

                            Console.Write("Enter the day of birth:\n=>");
                            string day = Console.ReadLine();

                            check = player.ChangeDateOfBirth(year, month, day);
                            if (check)
                            {
                                Console.WriteLine("Settings updated!");
                                PressKey();
                            }
                        }
                        else break;
                    }
                }
            }
        }

        private void AddQuizToHistory(string login, string themeName,int score,DateTime timeStart,DateTime timeEnd)
        {
            QuizzesHistory history = new QuizzesHistory();
            history.NamePlayer = login;
            history.NameTheme = themeName;
            history.Score = score;
            history.dateOfStart = timeStart;
            history.dateOfEnd = timeEnd;
            historyOfAllQuizzes.Add(history);
        }

        private void PressKey()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

    }
}

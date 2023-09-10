using System.Xml.Linq;

namespace Quiz
{
    public class Quiz : LINQFileWorker
    {
        public string NameTheme { get; set; }
        public string Description { get; set; }

        private static string path = "Themes.xml";

        private Dictionary<string, List<string>> questions;

        public Quiz(string name, string description)
        {
            NameTheme = name;
            Description = description;
            questions = new Dictionary<string, List<string>>();

            if (!File.Exists(path)) SaveList(null);
        }

        public Quiz() : this("", "") { }

        public static bool SearchQuiz(string nameTheme, List<Quiz> quizzes, ref Quiz quiz)
        {
            foreach (var item in quizzes)
            {
                if (item.NameTheme.ToLower() == nameTheme.ToLower())
                {
                    quiz = item;
                    return true;
                }
            }
            return false;
        }

        public static void AddUpdateToFile(List<Quiz> quizzes)
        {
            SaveList(quizzes);
        }

        public static List<Quiz> FileToList()
        {
            List<Quiz> quizzes = new List<Quiz>();
            var themes = LINQFileWorker.ReadFile(path, "Theme");

            if (themes == null) return quizzes;

            foreach (var theme in themes)
            {
                Quiz quiz = new Quiz();
                quiz.NameTheme = theme.Attribute("NameTheme").Value;
                quiz.Description = theme.Element("Description").Value;
                quiz.questions = new Dictionary<string, List<string>>();
                foreach (var item in theme.Elements("TaskBlock").Elements("Task"))
                {
                    List<string> list = new List<string>();
                    foreach (var answer in item.Elements("Answers").Elements("Answer"))
                    {
                        list.Add(answer.Value);
                    }
                    quiz.questions.Add(item.Element("Question").Value, list);
                }
                quizzes.Add(quiz);
            }
            return quizzes;
        }

        public bool CheckQuestionInList(string question)
        {
            foreach (var item in questions)
            {
                if(item.Key==question) return true;
            }
            return false;
        }

        public string GetQuestionRandomTheme(List<string> lastQuestions, List<Quiz> quizzes)
        {
            Random random = new Random();
            int allSize = 0;

            foreach(var item in quizzes)
            {
                allSize += item.questions.Count;
            }

            while (lastQuestions.Count != 20 && lastQuestions.Count!=allSize)
            {
                int index=random.Next(0, quizzes.Count);

                foreach (var i in quizzes[index].questions)
                {
                    if (lastQuestions.IndexOf(i.Key) == -1) return i.Key;
                }
            }
            return null;
        }

        public string GetQuestion(List<string> lastQuestions)
        {
            while (lastQuestions.Count != questions.Count)
            {
                foreach (var i in questions)
                {
                    if (lastQuestions.IndexOf(i.Key) == -1) return i.Key;
                }
            }
            return null;
        }

        public static bool CheckAnswer(string questionStr, string answer, List<Quiz> quizzes)
        {
            foreach (var quiz in quizzes)
            {
                foreach (var question in quiz.questions)
                {
                    if (question.Key != questionStr) continue;

                    foreach (var item in question.Value)
                    {
                        if (item.ToLower() == answer.ToLower()) return true;
                    }
                }

            }
                return false;
        }

        public bool CheckAnswer(string questionStr, string answer)
        {
            foreach (var question in questions)
            {
                if (question.Key != questionStr) continue;

                foreach (var item in question.Value)
                {
                    if (item.ToLower() == answer.ToLower()) return true;
                }
            }
            return false;
        }

        public static void AddXElement(Quiz themeGame)
        {
            var document = XDocument.Load(path);

            document.Element("Themes").Add(CreateXElement(themeGame));

            document.Save(path);
        }

        public void AddQuestion(string question, List<string> answer)
        {
            questions.Add(question, answer);
        }

        public void RemoveQuestion(string question)
        {
            questions.Remove(question);
        }

        private static XElement CreateXElementQuestions(Quiz theme)
        {
            var elementLINQ=new XElement("TaskBlock");
            
            foreach(var item in theme.questions)
            {
                var taskLINQ = new XElement("Task");
                var questionsLINQ = new XElement("Question");
                var answersLINQ = new XElement("Answers");
                questionsLINQ.Add(new XText(item.Key));
                foreach (var answer in item.Value)
                {
                    var answerLINQ = new XElement("Answer");
                    answerLINQ.Add(new XText(answer));
                    answersLINQ.Add(answerLINQ);
                }
                taskLINQ.Add(questionsLINQ);
                taskLINQ.Add(answersLINQ);
                elementLINQ.Add(taskLINQ);
            }
            return elementLINQ;
        }

        private static XElement CreateXElement(Quiz theme)
        {
            var elementLINQ = new XElement("Theme");
            elementLINQ.Add(new XAttribute("NameTheme", theme.NameTheme));
            elementLINQ.Add(new XElement("Description", theme.Description));
            elementLINQ.Add(CreateXElementQuestions(theme));
            return elementLINQ;
        }

        private static void SaveList(List<Quiz> objects)
        {
            var objectLINQ = new XElement("Themes");

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
    }
}

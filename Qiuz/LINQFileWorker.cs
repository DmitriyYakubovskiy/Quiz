using System.Xml.Linq;

namespace Quiz
{
    public abstract class LINQFileWorker
    {
        public static IEnumerable<XElement> ReadFile(string path,string nameDescendants)
        {
            var document = XDocument.Load(path);
            var elements = document.Descendants(nameDescendants);
            return elements;
        }

        protected IEnumerable<XElement> DeleteXElement(string name, string nameElement, string path, string nameDescendants)
        {
            var objects = ReadFile(path, nameDescendants);
            objects = objects.Where(x => x.Attribute(nameElement).Value != name);
            return objects;
        }

    }
}

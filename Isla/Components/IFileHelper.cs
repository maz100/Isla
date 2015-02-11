using System.IO;

namespace Isla.Components
{
    public interface IFileHelper
    {
        string[] ReadAllLines(string path);
        string ReadAllText(string path);
        void WriteAllLines(string path, string[] lines);
        bool Exists(string path);
        FileStream Create(string path);
    }
}
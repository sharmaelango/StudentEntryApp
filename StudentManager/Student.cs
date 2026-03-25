using System.Xml.Serialization;

namespace StudentManager
{
    [Serializable]
    public class Student
    {
        public int SNo { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string Class { get; set; } = string.Empty;

        public string RegisterNumber { get; set; } = string.Empty;
    }
}
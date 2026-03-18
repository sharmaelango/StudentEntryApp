using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace StudentManager
{
    public class Student
    {
        public int SNo { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string RegisterNumber { get; set; } = string.Empty;
    }
}
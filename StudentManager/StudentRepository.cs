using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace StudentManager
{
    public class StudentRepository
    {

        private readonly string _filePath;

        public StudentRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<Student> LoadAll()
        {
            if (!File.Exists(_filePath))
                return new List<Student>();

            try
            {
                var serializer = new XmlSerializer(typeof(List<Student>));
                using var reader = new StreamReader(_filePath);
                var result = serializer.Deserialize(reader) as List<Student>;
                return result ?? new List<Student>();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to load from:\n{_filePath}\n\n{ex.Message}");
            }
        }

        public void SaveAll(List<Student> students)
        {
            try
            {
                // Create folder if it does not exist
                string? directory = System.IO.Path.GetDirectoryName(_filePath);
                if (directory != null)
                    Directory.CreateDirectory(directory);

                var serializer = new XmlSerializer(typeof(List<Student>));
                using var writer = new StreamWriter(_filePath);
                serializer.Serialize(writer, students);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to save to:\n{_filePath}\n\n{ex.Message}");
            }
        }

        public bool FileExists()
        {
            return File.Exists(_filePath);
        }
    }
}

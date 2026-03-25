
using System.Collections.ObjectModel;

namespace StudentManager
{
    public class StudentManagerService
    {
 
        private readonly StudentRepository _repository;

        public ObservableCollection<Student> Students { get; private set; }
            = new ObservableCollection<Student>();
        public StudentManagerService(string dataFilePath)
        {
            _repository = new StudentRepository(dataFilePath);
        }
        public void Load()
        {
            var loaded = _repository.LoadAll();
            foreach (var student in loaded)
                Students.Add(student);
        }
        public void Save()
        {
            _repository.SaveAll(Students.ToList());
        }
        public Student AddStudent(string name, string className,
                                  string registerNumber)
        {
            var newStudent = new Student
            {
                SNo = Students.Count + 1,
                StudentName = name.Trim(),
                Class = className.Trim(),
                RegisterNumber = registerNumber.Trim()
            };

            Students.Add(newStudent);
            Save();
            return newStudent;
        }
        public void UpdateStudent(Student student, string name,
                                  string className, string registerNumber)
        {
            student.StudentName = name.Trim();
            student.Class = className.Trim();
            student.RegisterNumber = registerNumber.Trim();
            Save();
        }
        public void DeleteStudent(Student student)
        {
            Students.Remove(student);
            Renumber();
            Save();
        }

        public void Renumber()
        {
            for (int i = 0; i < Students.Count; i++)
                Students[i].SNo = i + 1;
        }


        public int GetNextSNo()
        {
            return Students.Count + 1;
        }


        public bool IsDuplicateRegisterNumber(string registerNumber,
                                              Student? excludeStudent = null)
        {
            return Students.Any(s =>
                s.RegisterNumber.Equals(registerNumber.Trim(),
                    StringComparison.OrdinalIgnoreCase) &&
                s != excludeStudent);
        }
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace StudentManager
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(name));

        private readonly StudentManagerService _manager;

        private string _sNo = string.Empty;
        private string _studentName = string.Empty;
        private string _className = string.Empty;
        private string _registerNumber = string.Empty;
        private string _statusMessage = "Ready";
        private bool _isError = false;
        private bool _isEditing = false;
        private Student? _editingStudent = null;
        private Student? _selectedStudent = null;
        public string SNo
        {
            get => _sNo;
            set { _sNo = value; OnPropertyChanged(); }
        }
        public string StudentName
        {
            get => _studentName;
            set { _studentName = value; OnPropertyChanged(); }
        }
        public string ClassName
        {
            get => _className;
            set { _className = value; OnPropertyChanged(); }
        }
        public string RegisterNumber
        {
            get => _registerNumber;
            set { _registerNumber = value; OnPropertyChanged(); }
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }
        public bool IsError
        {
            get => _isError;
            set { _isError = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Student> Students
            => _manager.Students;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();

                if (_selectedStudent != null)
                    LoadStudentIntoForm(_selectedStudent);
            }
        }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public StudentViewModel()
        {
            string dataFilePath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..", "..", "..",
                    "students.xml"
                )
            );
            _manager = new StudentManagerService(dataFilePath);
            try
            {
                _manager.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            AddCommand = new RelayCommand(
                execute: ExecuteAdd,
                canExecute: () => !_isEditing
            );
            UpdateCommand = new RelayCommand(
                execute: ExecuteUpdate,
                canExecute: () => _isEditing
            );
            DeleteCommand = new RelayCommand(
                execute: ExecuteDelete,
                canExecute: () => SelectedStudent != null
            );
            ClearCommand = new RelayCommand(
                execute: ExecuteClear
            );
            RefreshSNo();
        }
        private void ExecuteAdd()
        {
            if (!ValidateInputs()) return;

            if (_manager.IsDuplicateRegisterNumber(RegisterNumber))
            {
                ShowStatus("⚠ This register number already exists.", isError: true);
                return;
            }
            try
            {
                var added = _manager.AddStudent(
                    StudentName, ClassName, RegisterNumber);

                ShowStatus($"✔ '{added.StudentName}' added as S.No {added.SNo}.");
                ExecuteClear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteUpdate()
        {
            if (_editingStudent == null) return;
            if (!ValidateInputs()) return;

            if (_manager.IsDuplicateRegisterNumber(
                    RegisterNumber, _editingStudent))
            {
                ShowStatus("⚠ This register number already exists.", isError: true);
                return;
            }
            try
            {
                _manager.UpdateStudent(
                    _editingStudent,
                    StudentName,
                    ClassName,
                    RegisterNumber
                );

                ShowStatus($"✔ S.No {_editingStudent.SNo} updated successfully.");
                ExecuteClear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteDelete()
        {
            if (SelectedStudent == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete:\n\n" +
                $"  Name        : {SelectedStudent.StudentName}\n" +
                $"  Register No : {SelectedStudent.RegisterNumber}",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _manager.DeleteStudent(SelectedStudent);
                    ShowStatus("🗑 Student deleted successfully.");
                    ExecuteClear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Delete Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ExecuteClear()
        {
            StudentName = string.Empty;
            ClassName = string.Empty;
            RegisterNumber = string.Empty;

            _isEditing = false;
            _editingStudent = null;
            SelectedStudent = null;

            RefreshSNo();
            ShowStatus("Ready");
        }
        private void LoadStudentIntoForm(Student student)
        {
            SNo = student.SNo.ToString();
            StudentName = student.StudentName;
            ClassName = student.Class;
            RegisterNumber = student.RegisterNumber;

            _isEditing = true;
            _editingStudent = student;

            ShowStatus(
                $"Editing S.No {student.SNo} — update fields and click Update.");
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(StudentName))
            {
                ShowStatus("⚠ Student name is required.", isError: true);
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClassName))
            {
                ShowStatus("⚠ Class is required.", isError: true);
                return false;
            }

            if (string.IsNullOrWhiteSpace(RegisterNumber))
            {
                ShowStatus("⚠ Register number is required.", isError: true);
                return false;
            }

            return true;
        }
        private void RefreshSNo()
        {
            SNo = _manager.GetNextSNo().ToString();
        }
        private void ShowStatus(string message, bool isError = false)
        {
            StatusMessage = message;
            IsError = isError;
        }
    }
}
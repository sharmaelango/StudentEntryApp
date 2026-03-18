using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;


namespace StudentManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Student> _students = new();

        private bool _isEditing = false;

        private Student? _editingStudent = null;

        private readonly string _dataFilePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "..",
            "..",
            "..",
            "students.xml"));
        public MainWindow()
        {
            InitializeComponent();

            StudentGrid.ItemsSource = _students;

            LoadStudents();

            RefreshSNo();

        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtClass.Text) ||
                string.IsNullOrWhiteSpace(txtRegNo.Text))
            {
                ShowStatus("Please Fill in all fields before submitting", isError: true);
                return;
            }
            if (_isEditing && _editingStudent != null)
            {
                _editingStudent.StudentName = txtName.Text.Trim();
                _editingStudent.Class = txtClass.Text.Trim();
                _editingStudent.RegisterNumber = txtRegNo.Text.Trim();

                StudentGrid.Items.Refresh();


                ShowStatus($"✔ S.No {_editingStudent.SNo} - record updated sucessfully.");
            }
            else
            {
                var newStudent = new Student
                {
                    SNo = _students.Count + 1,
                    StudentName = txtName.Text.Trim(),
                    Class = txtClass.Text.Trim(),
                    RegisterNumber = txtRegNo.Text.Trim()
                };
                _students.Add(newStudent);

                ShowStatus($"✔ S.No {newStudent.SNo} - record added sucessfully.");
            }
            SaveStudents();
            ClearForm();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (StudentGrid.SelectedItem is not Student selected)
            {
                ShowStatus("⚠ Please select a row from the table to delete.", isError: true);
                return;

            }

            var result = MessageBox.Show(
                $"Are you sure you want to detete the record for:\n\n" +
                $"  Name : {selected.StudentName}\n" +
                $"  Register No : {selected.RegisterNumber}",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _students.Remove(selected);

                RenumberStudents();

                SaveStudents();
                ClearForm();
                ShowStatus("Student record deleted successfully.");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            ShowStatus("Form cleared.");
        }

        private void StudentGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentGrid.SelectedItem is Student Selected)
            {
                txtSNo.Text = Selected.SNo.ToString();
                txtName.Text = Selected.StudentName;
                txtClass.Text = Selected.Class;
                txtRegNo.Text = Selected.RegisterNumber;

                _isEditing = true;
                _editingStudent = Selected;

                BtnAdd.Content = "Update Student";

                ShowStatus($"Editing S.No {Selected.SNo} - update fields and click Update.");
            }
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtClass.Text = string.Empty;
            txtRegNo.Text = string.Empty;
            _isEditing = false;
            _editingStudent = null;
            StudentGrid.SelectedItem = null;
            BtnAdd.Content = "Add Student";

            RefreshSNo();
        }

        private void RefreshSNo()
        {
            txtSNo.Text = (_students.Count + 1).ToString();
        }

        private void RenumberStudents()
        {
            for (int i = 0; i < _students.Count; i++)
            {
                _students[i].SNo = i + 1;
            }
            StudentGrid.Items.Refresh();

            RefreshSNo();
        }
        private void ShowStatus(string message, bool isError = false)
        {
            txtStatus.Text = message;
            txtStatus.Foreground = isError
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Green;
        }

        private void SaveStudents()
        {
            try
            {
                string? directory = System.IO.Path.GetDirectoryName(_dataFilePath);
                if (directory != null)
                { Directory.CreateDirectory(directory); }
                var serializer = new XmlSerializer(typeof(List<Student>));

                using var writer = new StreamWriter(_dataFilePath);
                serializer.Serialize(writer, _students.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while saving data:\n\n{ex.Message}",
                    "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    
    private void LoadStudents()
        {
            if (!File.Exists(_dataFilePath))
            {
                return;
            }
            try
            {
                var serializer = new XmlSerializer(typeof(List<Student>));
                using var reader = new StreamReader(_dataFilePath);
                var loaded = serializer.Deserialize(reader) as List<Student>;

                if (loaded != null)
                {
                    foreach (var student in loaded)
                    {
                        _students.Add(student);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while loading data:\n\n{ex.Message}",
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
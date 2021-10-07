using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace Wpf_LinqToSQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DataClasses1DataContext dataContext;
        public MainWindow()
        {
            InitializeComponent();
            //Connect to database server
            string connectionString = ConfigurationManager.ConnectionStrings["wpf_LinqToSQL.Properties.Settings.LongDBConnectionString"].ConnectionString;
            dataContext = new DataClasses1DataContext(connectionString);

            DeleteBark();
        }

        public void InsertUniversity()
        {
            //Delete existing Uni's
            dataContext.ExecuteCommand("delete from University");
            //Get University Class from DataClasses1.dbml
            University yale = new University();
            yale.Name = "Yale";
            dataContext.Universities.InsertOnSubmit(yale);
            University princeton = new University();
            princeton.Name = "Princeton";
            dataContext.Universities.InsertOnSubmit(princeton);

            dataContext.SubmitChanges();
            //Populate Object in xaml with name MainDataGrid
            MainDataGrid.ItemsSource = dataContext.Universities;
        }

        public void InsertStudent()
        {
            dataContext.ExecuteCommand("delete from Student");

            University yale = dataContext.Universities.First(un => un.Name.Equals("Yale"));
            University princeton = dataContext.Universities.First(un => un.Name.Equals("Princeton"));
            //Create list and insert multiple students into it
            List<Student> students = new List<Student>();

            students.Add(new Student
            {
                Name = "Carla",
                Gender = "Female",
                UniversityId = yale.Id
            });

            students.Add(new Student
            {
                Name = "Toni",
                Gender = "Male",
                University = yale
            });

            students.Add(new Student
            {
                Name = "Mark",
                Gender = "Male",
                University = princeton
            });

            students.Add(new Student
            {
                Name = "Hanna",
                Gender = "Female",
                University = princeton
            });

            dataContext.Students.InsertAllOnSubmit(students);

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Students;
        }

        public void InsertLectures()
        {
            dataContext.Lectures.InsertOnSubmit(new Lecture
            {
                Name = "Math"
            });

            dataContext.Lectures.InsertOnSubmit(new Lecture
            {
                Name = "History"
            });

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Lectures;
        }

        public void InsertStudentLectureAssociation()
        {
            //Get each students by their name
            Student Carla = dataContext.Students.First(st => st.Name.Equals("Carla"));
            Student Toni = dataContext.Students.First(st => st.Name.Equals("Toni"));
            Student Mark = dataContext.Students.First(st => st.Name.Equals("Mark"));
            Student Hanna = dataContext.Students.First(st => st.Name.Equals("Hanna"));
            //Get each lectures by name
            Lecture Math = dataContext.Lectures.First(lc => lc.Name.Equals("Math"));
            Lecture History = dataContext.Lectures.First(lc => lc.Name.Equals("History"));
            //Insert student and lecture into StudentLecture table
            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture
            {
                Student = Carla,
                Lecture = Math
            });

            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture
            {
                Student = Toni,
                Lecture = History
            });
            //Another way of inserting into StudentLecture
            StudentLecture slMark = new StudentLecture();
            slMark.Student = Mark;
            slMark.Lecture = History;
            dataContext.StudentLectures.InsertOnSubmit(slMark);

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.StudentLectures;
        }

        public void GetUniversityofMark() //Get the Uni of a specific person
        {
            Student Mark = dataContext.Students.First(st => st.Name.Equals("Mark"));

            University MarkUni = Mark.University;

            List<University> universities = new List<University>();
            universities.Add(MarkUni);

            MainDataGrid.ItemsSource = universities;
        }

        public void GetLectureOfMark() //Get the lecture of a specific person
        {
            Student Mark = dataContext.Students.First(st => st.Name.Equals("Mark"));

            var markLectures = from sl in Mark.StudentLectures select sl.Lecture;

            MainDataGrid.ItemsSource = markLectures;
        }

        public void GetAllStudentsFromYale() //Get all the students from a specific Uni
        {
            var studentsFromYale = from student in dataContext.Students
                                   where student.University.Name == "Yale"
                                   select student;
            MainDataGrid.ItemsSource = studentsFromYale;
        }

        public void GetAllMaleStudents() //Get all male students from all Uni
        {
            var maleStudents = from student in dataContext.Students
                               join university in dataContext.Universities
                               on student.University equals university
                               where student.Gender == "Male"
                               select university;

            MainDataGrid.ItemsSource = maleStudents;
        }

        public void GetAllLecturesFromYale()
        {
            var lecturesFromYale = from sl in dataContext.StudentLectures
                                   join student in dataContext.Students on sl.StudentId equals student.Id
                                   where student.University.Name == "Yale"
                                   select sl.Lecture;

            MainDataGrid.ItemsSource = lecturesFromYale;
        }

        public void updateMark() //Update data
        {
            Student Mark = dataContext.Students.FirstOrDefault(st => st.Name == "Mark");

            Mark.Name = "Bark";

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Students;
        } 

        public void DeleteBark() //Delete data
        {
            Student Bark = dataContext.Students.FirstOrDefault(st => st.Name == "Bark");

            dataContext.Students.DeleteOnSubmit(Bark);

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Students;
        }
    }
}

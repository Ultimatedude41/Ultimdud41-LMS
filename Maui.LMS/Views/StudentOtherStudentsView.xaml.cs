using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentOtherStudentsView : ContentPage
{
	private Student currentStudent;
	private Course currentCourse;

	public int StudentId { get; set; }
	public int CourseId { get; set; }

	public StudentOtherStudentsView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentStudent = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == StudentId);
		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse != null && currentStudent != null)
		{
			// Get all students except current student
			var otherStudents = currentCourse.Students.Where(s => s.Id != StudentId).ToList();
			StudentList.ItemsSource = otherStudents;
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentCourseDetail?studentId={StudentId}&courseId={CourseId}");
	}
}
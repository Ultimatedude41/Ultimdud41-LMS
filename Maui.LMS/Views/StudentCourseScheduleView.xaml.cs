using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentCourseScheduleView : ContentPage
{
	private Student currentStudent;
	private Course currentCourse;

	public int StudentId { get; set; }
	public int CourseId { get; set; }

	public StudentCourseScheduleView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		ScheduleContainer.Clear();

		currentStudent = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == StudentId);
		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse != null)
		{
			LoadSchedule();
		}
	}

	private void LoadSchedule()
	{
		// Sort assignments by due date
		var sortedAssignments = currentCourse.Assignments.OrderBy(a => a.DueDate).ToList();

		foreach (var assignment in sortedAssignments)
		{
			var assignmentLabel = new Label
			{
				Text = $"[{assignment.DueDate}] {assignment.Name} - {assignment.AvailablePoints} points",
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};
			ScheduleContainer.Add(assignmentLabel);
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentCourseDetail?studentId={StudentId}&courseId={CourseId}");
	}
}
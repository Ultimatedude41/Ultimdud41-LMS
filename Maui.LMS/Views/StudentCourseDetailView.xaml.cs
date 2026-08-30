using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentCourseDetailView : ContentPage
{
	private CLI.LMS.Model.Student currentStudent;
	private Course currentCourse;

	public int StudentId { get; set; }
	public int CourseId { get; set; }

	public StudentCourseDetailView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentStudent = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == StudentId);
		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse != null)
		{
			CourseNameLabel.Text = $"[{currentCourse.Code}] {currentCourse.Name} ({currentCourse.Section}) - {currentCourse.Semester}";
			LetterGradeLabel.Text = $"Current Grade: {CalculateLetterGrade()}";

			LoadAnnouncements();
		}
	}

	private void LoadAnnouncements()
	{
		AnnouncementsContainer.Clear();

		if (currentCourse?.Announcements != null && currentCourse.Announcements.Count > 0)
		{
			foreach (var announcement in currentCourse.Announcements)
			{
				var frame = new Frame
				{
					BorderColor = Colors.Gray,
					CornerRadius = 5,
					Padding = 10,
					Margin = new Thickness(0, 5, 0, 5)
				};

				var stackLayout = new VerticalStackLayout();

				var titleLabel = new Label
				{
					Text = announcement.Title,
					FontSize = 16,
					FontAttributes = FontAttributes.Bold
				};

				var dateLabel = new Label
				{
					Text = $"Posted: {announcement.PostDate}",
					FontSize = 12,
					TextColor = Colors.Gray
				};

				var messageLabel = new Label
				{
					Text = announcement.Message,
					FontSize = 14
				};

				stackLayout.Add(titleLabel);
				stackLayout.Add(dateLabel);
				stackLayout.Add(messageLabel);

				frame.Content = stackLayout;
				AnnouncementsContainer.Add(frame);
			}
		}
	}

	private string CalculateLetterGrade()
	{
		if (currentStudent == null || currentCourse == null) return "N/A";

		double totalWeightedScore = 0;
		double totalWeight = 0;

		foreach (var agroup in currentCourse.AGroups)
		{
			double groupScore = 0;
			double groupMaxPoints = 0;

			foreach (var assignment in agroup.Assignments)
			{
				var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == currentStudent.Id);
				if (submission != null && submission.Grade.HasValue)
				{
					groupScore += submission.Grade.Value;
					groupMaxPoints += assignment.AvailablePoints;
				}
			}

			if (groupMaxPoints > 0)
			{
				double groupPercentage = (groupScore / groupMaxPoints) * 100;
				totalWeightedScore += groupPercentage * agroup.Weight;
				totalWeight += agroup.Weight;
			}
		}

		if (totalWeight == 0) return "N/A";

		double finalGrade = totalWeightedScore / totalWeight;

		// Use course-specific grade ranges
		if (finalGrade >= currentCourse.GradeRangeA) return "A";
		if (finalGrade >= currentCourse.GradeRangeB) return "B";
		if (finalGrade >= currentCourse.GradeRangeC) return "C";
		if (finalGrade >= currentCourse.GradeRangeD) return "D";
		return "F";
	}

	private void OnViewModulesClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentModules?studentId={StudentId}&courseId={CourseId}");
	}

	private void OnViewAssignmentsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentAssignments?studentId={StudentId}&courseId={CourseId}");
	}

	private void OnViewGradesClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentGrades?studentId={StudentId}&courseId={CourseId}");
	}

	private void OnViewOtherStudentsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentOtherStudents?studentId={StudentId}&courseId={CourseId}");
	}

	private void OnViewCourseScheduleClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentCourseSchedule?studentId={StudentId}&courseId={CourseId}");
	}

	private void OnUnenrollClicked(object sender, EventArgs e)
	{
		// Remove course from student's enrolled courses
		currentCourse.Students.Remove(currentStudent);

		// Remove student from course's student list
		currentCourse.Students.Remove(currentStudent);

		// Go back to course list
		Shell.Current.GoToAsync($"//StudentCourses?studentId={StudentId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentCourses?studentId={StudentId}");
	}
}
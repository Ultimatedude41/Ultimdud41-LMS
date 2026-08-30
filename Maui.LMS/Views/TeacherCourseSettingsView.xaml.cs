using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherCourseSettingsView : ContentPage
{
	private Course currentCourse;

	public int CourseId { get; set; }

	public TeacherCourseSettingsView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse != null)
		{
			CourseNameLabel.Text = $"Settings for: {currentCourse.Name}";

			// Load current grade ranges
			AGradeEntry.Text = currentCourse.GradeRangeA.ToString();
			BGradeEntry.Text = currentCourse.GradeRangeB.ToString();
			CGradeEntry.Text = currentCourse.GradeRangeC.ToString();
			DGradeEntry.Text = currentCourse.GradeRangeD.ToString();
		}
	}

	private void OnSaveSettingsClicked(object sender, EventArgs e)
	{
		if (currentCourse == null) return;

		// Validate and parse grade ranges
		if (!double.TryParse(AGradeEntry.Text, out double aGrade) ||
			!double.TryParse(BGradeEntry.Text, out double bGrade) ||
			!double.TryParse(CGradeEntry.Text, out double cGrade) ||
			!double.TryParse(DGradeEntry.Text, out double dGrade))
		{
			// Invalid input - could show error message
			return;
		}

		// Validate ranges (A should be highest, D should be lowest)
		if (aGrade <= bGrade || bGrade <= cGrade || cGrade <= dGrade || dGrade < 0)
		{
			// Invalid ranges - could show error message
			return;
		}

		// Save grade ranges
		currentCourse.GradeRangeA = aGrade;
		currentCourse.GradeRangeB = bGrade;
		currentCourse.GradeRangeC = cGrade;
		currentCourse.GradeRangeD = dGrade;

		// Navigate back
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}

	private void OnResetToDefaultsClicked(object sender, EventArgs e)
	{
		// Reset to default values
		AGradeEntry.Text = "90";
		BGradeEntry.Text = "80";
		CGradeEntry.Text = "70";
		DGradeEntry.Text = "60";
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}
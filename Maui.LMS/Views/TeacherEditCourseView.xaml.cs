using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherEditCourseView : ContentPage
{
	private Course currentCourse;

	public int CourseId { get; set; }

	public TeacherEditCourseView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse != null)
		{
			CodeEntry.Text = currentCourse.Code;
			NameEntry.Text = currentCourse.Name;
			SectionEntry.Text = currentCourse.Section;
			DescriptionEditor.Text = currentCourse.Description;
			SemesterEntry.Text = currentCourse.Semester;
		}
	}

	private void OnSaveChangesClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(CodeEntry.Text) ||
			string.IsNullOrWhiteSpace(NameEntry.Text) ||
			string.IsNullOrWhiteSpace(SectionEntry.Text) ||
			string.IsNullOrWhiteSpace(DescriptionEditor.Text) ||
			string.IsNullOrWhiteSpace(SemesterEntry.Text))
		{
			return;
		}

		if (currentCourse != null)
		{
			currentCourse.Code = CodeEntry.Text;
			currentCourse.Name = NameEntry.Text;
			currentCourse.Section = SectionEntry.Text;
			currentCourse.Description = DescriptionEditor.Text;
			currentCourse.Semester = SemesterEntry.Text;
		}

		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}
}
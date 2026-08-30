using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

public partial class TeacherAddCourseView : ContentPage
{
	public TeacherAddCourseView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		CodeEntry.Text = string.Empty;
		NameEntry.Text = string.Empty;
		SectionEntry.Text = string.Empty;
		DescriptionEditor.Text = string.Empty;
		SemesterEntry.Text = string.Empty;
	}

	private void OnCreateCourseClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(CodeEntry.Text) ||
			string.IsNullOrWhiteSpace(NameEntry.Text) ||
			string.IsNullOrWhiteSpace(SectionEntry.Text) ||
			string.IsNullOrWhiteSpace(DescriptionEditor.Text) ||
			string.IsNullOrWhiteSpace(SemesterEntry.Text))
		{
			return;
		}

		var newCourse = new Course
		{
			Code = CodeEntry.Text,
			Name = NameEntry.Text,
			Section = SectionEntry.Text,
			Description = DescriptionEditor.Text,
			Semester = SemesterEntry.Text
		};

		CourseServiceProxy.Current.Add(newCourse);

		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}
}
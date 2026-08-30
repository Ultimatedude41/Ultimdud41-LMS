namespace Maui.LMS.Views;

public partial class TeacherMainView : ContentPage
{
	public TeacherMainView()
	{
		InitializeComponent();
	}

	private void OnManageCoursesClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}

	private void OnManageStudentsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherStudentManagement");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//MainPage");
	}
}
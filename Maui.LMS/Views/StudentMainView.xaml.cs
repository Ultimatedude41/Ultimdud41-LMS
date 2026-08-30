using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

public partial class StudentMainView : ContentPage
{
	private StudentMainViewViewModel viewModel;

	public StudentMainView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		BindingContext = null;
		viewModel = new StudentMainViewViewModel();
		BindingContext = viewModel;
	}

	private void OnStudentClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var studentId = viewModel.GetStudentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//StudentCourses?studentId={studentId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		 Shell.Current.GoToAsync("//MainPage");
	}
}
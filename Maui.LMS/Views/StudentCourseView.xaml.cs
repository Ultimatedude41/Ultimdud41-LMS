using Library.LMS.Services;
using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentCourseView : ContentPage
{
	private StudentCourseViewViewModel viewModel;

	public int StudentId { get; set; }

	public StudentCourseView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		// Clear and recreate ViewModel - bindings automatically update UI
		BindingContext = null;
		viewModel = new StudentCourseViewViewModel(StudentId);
		BindingContext = viewModel;
	}

	private void OnCourseClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var courseId = viewModel.GetCourseId(button.CommandParameter);

		Shell.Current.GoToAsync($"//StudentCourseDetail?studentId={StudentId}&courseId={courseId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//StudentMenu");
	}
}
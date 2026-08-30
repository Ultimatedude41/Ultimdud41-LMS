using CLI.LMS.Model;
using Library.LMS.Services;
using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(TargetCourseId), "targetCourseId")]
public partial class TeacherCopyAssignmentView : ContentPage
{
	private TeacherCopyAssignmentViewViewModel viewModel;
	private Course selectedCourse;

	public int TargetCourseId { get; set; }

	public TeacherCopyAssignmentView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		viewModel = new TeacherCopyAssignmentViewViewModel(TargetCourseId);

		var courseCount = viewModel.OtherCourses?.Count ?? 0;

		BindingContext = null;
		BindingContext = viewModel;

		selectedCourse = null;
		AssignmentListView.ItemsSource = null;
		AssignmentSectionLabel.IsVisible = false;
		AssignmentListView.IsVisible = false;
	}

	private void OnCourseSelected(object sender, SelectedItemChangedEventArgs e)
	{
		selectedCourse = e.SelectedItem as Course;

		if (selectedCourse != null)
		{
			AssignmentSectionLabel.IsVisible = true;
			AssignmentListView.IsVisible = true;
			AssignmentListView.ItemsSource = selectedCourse.Assignments;
		}
	}

	private void OnCopyClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var originalAssignment = button.CommandParameter as Assignment;

		if (originalAssignment != null && viewModel.TargetCourse != null)
		{
			// Create copy of assignment (without submissions)
			var newAssignment = new Assignment
			{
				Name = originalAssignment.Name,
				Description = originalAssignment.Description,
				AvailablePoints = originalAssignment.AvailablePoints,
				DueDate = originalAssignment.DueDate
			};

			// Add to service and target course
			AssignmentServiceProxy.Current.Add(newAssignment);
			viewModel.TargetCourse.Assignments.Add(newAssignment);
		}

		// Navigate back
		Shell.Current.GoToAsync($"//TeacherManageAssignments?courseId={TargetCourseId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignments?courseId={TargetCourseId}");
	}
}
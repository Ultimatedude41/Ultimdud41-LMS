using CLI.LMS.Model;
using Library.LMS.Services;
using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(GroupId), "groupId")]
public partial class TeacherAddAssignmentToGroupView : ContentPage
{
	private TeacherAddAssignmentToGroupViewViewModel viewModel;

	public int CourseId { get; set; }
	public int GroupId { get; set; }

	public TeacherAddAssignmentToGroupView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		viewModel = new TeacherAddAssignmentToGroupViewViewModel(CourseId, GroupId);

		// Force property evaluation
		var assignmentCount = viewModel.AvailableAssignments?.Count ?? 0;

		if (viewModel.CurrentGroup != null)
		{
			GroupNameLabel.Text = $"Add Assignment to: {viewModel.CurrentGroup.Name}";
		}

		BindingContext = null;
		BindingContext = viewModel;

		bool hasAssignments = assignmentCount > 0;
		EmptyStateLabel.IsVisible = !hasAssignments;
		AssignmentListView.IsVisible = hasAssignments;
	}

	private void OnAddClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var assignment = button.CommandParameter as Assignment;

		if (assignment != null && viewModel.CurrentGroup != null)
		{
			viewModel.CurrentGroup.Assignments.Add(assignment);
		}

		Shell.Current.GoToAsync($"//TeacherManageAssignmentGroups?courseId={CourseId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignmentGroups?courseId={CourseId}");
	}
}
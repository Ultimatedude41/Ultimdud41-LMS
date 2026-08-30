using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentGradesView : ContentPage
{
	private StudentGradesViewViewModel viewModel;

	public int StudentId { get; set; }
	public int CourseId { get; set; }

	public StudentGradesView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		WeightedGradesContainer.Clear();

		viewModel = new StudentGradesViewViewModel(StudentId, CourseId);
		BindingContext = viewModel;

		LoadWeightedGrades();
	}

	private void LoadWeightedGrades()
	{
		foreach (var groupGrade in viewModel.WeightedGrades)
		{
			var groupLabel = new Label
			{
				Text = $"{groupGrade.GroupName} (Weight: {groupGrade.Weight * 100}%)",
				HorizontalOptions = LayoutOptions.Center,
				FontSize = 14
			};
			WeightedGradesContainer.Add(groupLabel);

			if (groupGrade.AssignmentGrades.Any())
			{
				foreach (var assignmentGrade in groupGrade.AssignmentGrades)
				{
					var gradeLabel = new Label
					{
						Text = assignmentGrade,
						HorizontalOptions = LayoutOptions.Center
					};
					WeightedGradesContainer.Add(gradeLabel);
				}
			}

			var summaryLabel = new Label
			{
				Text = groupGrade.GroupSummary,
				HorizontalOptions = LayoutOptions.Center
			};
			WeightedGradesContainer.Add(summaryLabel);
		}
	}

	private void OnViewDetailsClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var assignmentId = viewModel.GetAssignmentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//StudentGradeDetail?studentId={StudentId}&courseId={CourseId}&assignmentId={assignmentId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentCourseDetail?studentId={StudentId}&courseId={CourseId}");
	}
}
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssignmentId), "assignmentId")]
public partial class TeacherEditAssignmentView : ContentPage
{
	private Assignment currentAssignment;

	public int CourseId { get; set; }
	public int AssignmentId { get; set; }

	public TeacherEditAssignmentView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentAssignment = AssignmentServiceProxy.Current.Assignments.FirstOrDefault(a => a.Id == AssignmentId);

		if (currentAssignment != null)
		{
			NameEntry.Text = currentAssignment.Name;
			PointsEntry.Text = currentAssignment.AvailablePoints.ToString();
			DueDateEntry.Text = currentAssignment.DueDate;

			// Check if it's a quiz or regular assignment
			if (currentAssignment is Quiz quiz)
			{
				TypeLabel.Text = "Type: Quiz";
				QuestionSection.IsVisible = true;
				DescriptionSection.IsVisible = false;
				QuestionEditor.Text = quiz.Question;
			}
			else
			{
				TypeLabel.Text = "Type: Assignment";
				DescriptionSection.IsVisible = true;
				QuestionSection.IsVisible = false;
				DescriptionEditor.Text = currentAssignment.Description;
			}
		}
	}

	private void OnSaveClicked(object sender, EventArgs e)
	{
		// Validate input
		if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
			string.IsNullOrWhiteSpace(PointsEntry.Text) ||
			string.IsNullOrWhiteSpace(DueDateEntry.Text))
		{
			return;
		}

		if (!int.TryParse(PointsEntry.Text, out int points))
		{
			return;
		}

		// Update assignment properties
		if (currentAssignment != null)
		{
			currentAssignment.Name = NameEntry.Text;
			currentAssignment.AvailablePoints = points;
			currentAssignment.DueDate = DueDateEntry.Text;

			if (currentAssignment is Quiz quiz)
			{
				if (string.IsNullOrWhiteSpace(QuestionEditor.Text))
				{
					return;
				}
				quiz.Question = QuestionEditor.Text;
			}
			else
			{
				if (string.IsNullOrWhiteSpace(DescriptionEditor.Text))
				{
					return;
				}
				currentAssignment.Description = DescriptionEditor.Text;
			}
		}

		// Navigate back
		Shell.Current.GoToAsync($"//TeacherManageAssignments?courseId={CourseId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignments?courseId={CourseId}");
	}
}
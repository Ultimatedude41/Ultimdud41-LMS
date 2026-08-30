using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherAddAssignmentView : ContentPage
{
	public int CourseId { get; set; }

	public TeacherAddAssignmentView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		
		TypePicker.SelectedIndex = 0;
		NameEntry.Text = string.Empty;
		DescriptionEditor.Text = string.Empty;
		QuestionEditor.Text = string.Empty;
		PointsEntry.Text = string.Empty;
		DueDateEntry.Text = string.Empty;
		
		DescriptionSection.IsVisible = true;
		QuestionSection.IsVisible = false;
	}

	private void OnTypeChanged(object sender, EventArgs e)
	{
		// Hide all sections first
		DescriptionSection.IsVisible = false;
		QuestionSection.IsVisible = false;
		
		// Show relevant section based on selection
		switch (TypePicker.SelectedIndex)
		{
			case 0: // Assignment
				DescriptionSection.IsVisible = true;
				break;
			case 1: // Quiz
				QuestionSection.IsVisible = true;
				break;
		}
	}

	private void OnAddAssignmentClicked(object sender, EventArgs e)
	{
		// Validate common input
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

		if (!DateTime.TryParse(DueDateEntry.Text, out _))
		{
			return;
		}

		Assignment newAssignment = null;

		switch (TypePicker.SelectedIndex)
		{
			case 0: // Regular Assignment
				if (string.IsNullOrWhiteSpace(DescriptionEditor.Text))
				{
					return;
				}
				
				newAssignment = new Assignment
				{
					Name = NameEntry.Text,
					Description = DescriptionEditor.Text,
					AvailablePoints = points,
					DueDate = DueDateEntry.Text
				};
				break;

			case 1: // Quiz
				if (string.IsNullOrWhiteSpace(QuestionEditor.Text))
				{
					return;
				}
				
				newAssignment = new Quiz
				{
					Name = NameEntry.Text,
					Question = QuestionEditor.Text,
					Description = "", // Empty for quizzes
					AvailablePoints = points,
					DueDate = DueDateEntry.Text
				};
				break;
		}

		if (newAssignment != null)
		{
			// Add to service
			AssignmentServiceProxy.Current.Add(newAssignment);

			// Add to course
			var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);
			if (course != null)
			{
				course.Assignments.Add(newAssignment);
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
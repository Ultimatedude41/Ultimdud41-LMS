using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherManageAssignmentGroupsView : ContentPage
{
	private TeacherManageAssignmentGroupsViewViewModel viewModel;

	public int CourseId { get; set; }

	public TeacherManageAssignmentGroupsView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		RefreshData();
	}

	private void RefreshData()
	{
		viewModel = new TeacherManageAssignmentGroupsViewViewModel(CourseId);

		// Force property evaluation
		var groupCount = viewModel.AssignmentGroups?.Count ?? 0;

		if (viewModel.CurrentCourse != null)
		{
			CourseNameLabel.Text = $"{viewModel.CurrentCourse.Name} - Assignment Groups";
		}

		LoadGroups();

		bool hasGroups = groupCount > 0;
		EmptyStateLabel.IsVisible = !hasGroups;
	}

	private void LoadGroups()
	{
		GroupsContainer.Clear();

		foreach (var group in viewModel.AssignmentGroups)
		{
			// Group header with buttons
			var headerGrid = new Grid
			{
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto }
				},
				Margin = new Thickness(0, 10, 0, 5)
			};

			var groupLabel = new Label
			{
				Text = $"{group.Name} (Weight: {group.Weight}%)",
				FontSize = 16,
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center
			};

			var addAssignmentButton = new Button
			{
				Text = "Add Assignment",
				CommandParameter = group,
				VerticalOptions = LayoutOptions.Center
			};
			addAssignmentButton.Clicked += OnAddAssignmentToGroupClicked;

			var editButton = new Button
			{
				Text = "Edit",
				CommandParameter = group,
				VerticalOptions = LayoutOptions.Center
			};
			editButton.Clicked += OnEditGroupClicked;

			var deleteButton = new Button
			{
				Text = "Delete",
				CommandParameter = group,
				VerticalOptions = LayoutOptions.Center
			};
			deleteButton.Clicked += OnDeleteGroupClicked;

			headerGrid.Add(groupLabel, 0, 0);
			headerGrid.Add(addAssignmentButton, 1, 0);
			headerGrid.Add(editButton, 2, 0);
			headerGrid.Add(deleteButton, 3, 0);

			GroupsContainer.Add(headerGrid);

			// FILTER: Only show assignments that belong to THIS course
			var courseAssignments = group.Assignments
				.Where(a => viewModel.CurrentCourse.Assignments.Contains(a))
				.ToList();

			// List assignments in this group
			if (group.Assignments.Count > 0)
			{
				foreach (var assignment in group.Assignments)
				{
					var assignmentGrid = new Grid
					{
						ColumnDefinitions =
						{
							new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
							new ColumnDefinition { Width = GridLength.Auto }
						},
						Margin = new Thickness(20, 0, 0, 0)
					};

					var assignmentLabel = new Label
					{
						Text = $"  • {assignment.Name}",
						VerticalOptions = LayoutOptions.Center
					};

					var removeButton = new Button
					{
						Text = "Remove",
						CommandParameter = new Tuple<CLI.LMS.Model.AGroup, CLI.LMS.Model.Assignment>(group, assignment),
						VerticalOptions = LayoutOptions.Center
					};
					removeButton.Clicked += OnRemoveAssignmentFromGroupClicked;

					assignmentGrid.Add(assignmentLabel, 0, 0);
					assignmentGrid.Add(removeButton, 1, 0);

					GroupsContainer.Add(assignmentGrid);
				}
			}
			else
			{
				var emptyLabel = new Label
				{
					Text = "  (No assignments in this group)",
					FontSize = 12,
					TextColor = Colors.Gray,
					Margin = new Thickness(20, 0, 0, 0)
				};
				GroupsContainer.Add(emptyLabel);
			}

			// Add separator
			var separator = new BoxView
			{
				HeightRequest = 1,
				Color = Colors.LightGray,
				Margin = new Thickness(0, 5, 0, 0)
			};
			GroupsContainer.Add(separator);
		}
	}

	private void OnEditGroupClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var groupId = viewModel.GetGroupId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherEditAssignmentGroup?courseId={CourseId}&groupId={groupId}");
	}

	private void OnDeleteGroupClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.DeleteGroup(button.CommandParameter);

		RefreshData();
	}

	private void OnAddAssignmentToGroupClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var groupId = viewModel.GetGroupId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherAddAssignmentToGroup?courseId={CourseId}&groupId={groupId}");
	}

	private void OnRemoveAssignmentFromGroupClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var tuple = button.CommandParameter as Tuple<CLI.LMS.Model.AGroup, CLI.LMS.Model.Assignment>;

		if (tuple != null)
		{
			tuple.Item1.Assignments.Remove(tuple.Item2);
			RefreshData();
		}
	}

	private void OnAddGroupClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherAddAssignmentGroup?courseId={CourseId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}
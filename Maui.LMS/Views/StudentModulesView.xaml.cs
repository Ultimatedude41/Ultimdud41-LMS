using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentModulesView : ContentPage
{
	private StudentModulesViewViewModel viewModel;

	public int StudentId { get; set; }
	public int CourseId { get; set; }

	public StudentModulesView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		ModuleContainer.Clear();

		viewModel = new StudentModulesViewViewModel(StudentId, CourseId);

		// Force property evaluation BEFORE loading modules
		var moduleCount = viewModel.Modules?.Count ?? 0;

		LoadModules();
	}

	private void LoadModules()
	{
		foreach (var module in viewModel.Modules)
		{
			// Module header button (toggle)
			var moduleButton = new Button
			{
				Text = $"Module {module.Id}",
				HorizontalOptions = LayoutOptions.Fill
			};

			// Content ListView (initially hidden)
			var contentListView = new ListView
			{
				IsVisible = false,
				ItemsSource = module.Content,
				ItemTemplate = new DataTemplate(() =>
				{
					var grid = new Grid
					{
						ColumnDefinitions =
						{
							new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
							new ColumnDefinition { Width = GridLength.Auto }
						}
					};

					var label = new Label
					{
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.Start
					};
					label.SetBinding(Label.TextProperty, ".");

					var button = new Button
					{
						Text = "Open",
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.End,
						CommandParameter = module.Id  // Store moduleId in CommandParameter temporarily
					};
					button.SetBinding(Button.CommandParameterProperty, ".");
					button.Clicked += (s, e) => OnContentClicked(s, e, module.Id);  // Pass moduleId

					grid.Add(label, 0, 0);
					grid.Add(button, 1, 0);

					return new ViewCell { View = grid };
				})
			};

			// Toggle visibility when module button clicked
			moduleButton.Clicked += (s, e) =>
			{
				contentListView.IsVisible = !contentListView.IsVisible;
			};

			ModuleContainer.Add(moduleButton);
			ModuleContainer.Add(contentListView);
		}
	}

	private void OnContentClicked(object sender, EventArgs e, int moduleId)
	{
		var button = sender as Button;
		var contentId = viewModel.GetContentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//StudentModulesDetail?studentId={StudentId}&courseId={CourseId}&moduleId={moduleId}&contentId={contentId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentCourseDetail?studentId={StudentId}&courseId={CourseId}");
	}
}
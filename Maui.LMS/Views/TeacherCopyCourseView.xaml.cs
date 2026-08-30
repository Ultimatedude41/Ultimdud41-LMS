using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherCopyCourseView : ContentPage
{
	private Course originalCourse;

	public int CourseId { get; set; }

	public TeacherCopyCourseView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		originalCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (originalCourse != null)
		{
			OriginalCourseLabel.Text = $"Copying: {originalCourse}";
			SectionEntry.Text = string.Empty;
			SemesterEntry.Text = string.Empty;
		}
	}

	private void OnCopyCourseClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(SectionEntry.Text) ||
			string.IsNullOrWhiteSpace(SemesterEntry.Text))
		{
			return;
		}

		if (originalCourse == null) return;

		// Create new course with updated section and semester
		var newCourse = new Course
		{
			Code = originalCourse.Code,
			Name = originalCourse.Name,
			Section = SectionEntry.Text,
			Description = originalCourse.Description,
			Semester = SemesterEntry.Text
		};

		// Add to service
		CourseServiceProxy.Current.Add(newCourse);

		// Copy assignments (without submissions)
		foreach (var assignment in originalCourse.Assignments)
		{
			var newAssignment = new Assignment
			{
				Name = assignment.Name,
				Description = assignment.Description,
				AvailablePoints = assignment.AvailablePoints,
				DueDate = assignment.DueDate
			};

			AssignmentServiceProxy.Current.Add(newAssignment);
			newCourse.Assignments.Add(newAssignment);
		}

		// Copy modules and content
		foreach (var module in originalCourse.Modules)
		{
			var newModule = new Module();
			ModuleServiceProxy.Current.Add(newModule);

			// Copy content
			foreach (var content in module.Content)
			{
				ContentPlus newContent = null;

				if (content is CLI.LMS.Model.Page page)
				{
					newContent = new CLI.LMS.Model.Page
					{
						Name = page.Name,
						Content = page.Content
					};
				}
				else if (content is FilePlus file)
				{
					newContent = new FilePlus
					{
						Name = file.Name,
						FilePath = file.FilePath
					};
				}
				else if (content is AssignPlus assignPlus)
				{
					// Find the copied assignment by name
					var copiedAssignment = newCourse.Assignments.FirstOrDefault(a => a.Name == assignPlus.Assignment.Name);
					if (copiedAssignment != null)
					{
						newContent = new AssignPlus
						{
							Name = assignPlus.Name,
							Assignment = copiedAssignment
						};
					}
				}

				if (newContent != null)
				{
					newModule.Content.Add(newContent);
				}
			}

			newCourse.Modules.Add(newModule);
		}

		// Copy assignment groups
		foreach (var agroup in originalCourse.AGroups)
		{
			var newAGroup = new AGroup
			{
				Name = agroup.Name,
				Weight = agroup.Weight
			};

			AGroupServiceProxy.Current.Add(newAGroup);

			// Link copied assignments to the new group
			foreach (var assignment in agroup.Assignments)
			{
				var copiedAssignment = newCourse.Assignments.FirstOrDefault(a => a.Name == assignment.Name);
				if (copiedAssignment != null)
				{
					newAGroup.Assignments.Add(copiedAssignment);
				}
			}

			newCourse.AGroups.Add(newAGroup);
		}

		// Navigate back
		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}
}
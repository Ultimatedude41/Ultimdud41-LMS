using System;
using System.Linq;
using CLI.LMS.Model;
using CLI.LMS.Model.Plus;
using Library.LMS.Model;
using Library.LMS.Services;

namespace MyApp
{
	public static class TeacherMenuHelper
	{
		public static void ShowTeacherMenu()
		{
			bool exitTeacherMenu = false;

			while (!exitTeacherMenu)
			{
				Console.WriteLine("Teacher Menu");
				Console.WriteLine("A. Add New Course");
				Console.WriteLine("S. Select Course");
				Console.WriteLine("D. Delete Course");
				Console.WriteLine("C. Copy Course");
				Console.WriteLine("P. Exit");
				var subChoice = Console.ReadLine();

				if (subChoice.Equals("A", StringComparison.InvariantCultureIgnoreCase))
				{
					AddNewCourse();
				}
				else if (subChoice.Equals("S", StringComparison.InvariantCultureIgnoreCase))
				{
					SelectCourse();
				}
				else if (subChoice.Equals("D", StringComparison.InvariantCultureIgnoreCase))
				{
					DeleteCourse();
				}
				else if (subChoice.Equals("C", StringComparison.InvariantCultureIgnoreCase))
				{
					CopyCourse();
				}
				else if (subChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					Console.WriteLine("Exiting teacher menu...");
					exitTeacherMenu = true;
				}
			}
		}

		private static void AddNewCourse()
		{
			Console.WriteLine("Name:");
			var name = Console.ReadLine();
			Console.WriteLine("Code:");
			var code = Console.ReadLine();
			Console.WriteLine("Description:");
			var description = Console.ReadLine();
			Console.WriteLine("Semester (e.g. Fall 2025):");
			var semester = Console.ReadLine();
			Console.WriteLine("Section (e.g. A, B, 01, 02):");
			var section = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(semester))
			{
				Console.WriteLine("Semester is required. Course not created.");
				return;
			}

			var course = new Course
			{
				Name = name,
				Code = code,
				Description = description,
				Semester = semester,
				Section = section
			};

			CourseServiceProxy.Current.Add(course);
			Console.WriteLine(course);
		}

		private static void SelectCourse()
		{
			bool exitTeacherCourseSelection = false;
			while (!exitTeacherCourseSelection)
			{
				Console.WriteLine("\nAll Courses:");

				// Sort courses by semester, then by code, then by section
				var sortedCourses = CourseServiceProxy.Current.Courses
					.OrderBy(c => c.Semester)
					.ThenBy(c => c.Code)
					.ThenBy(c => c.Section)  // ADD THIS LINE
					.ToList();

				if (sortedCourses.Any())
				{
					string currentSemester = "";
					foreach (var course in sortedCourses)
					{
						// Print semester header when it changes
						if (course.Semester != currentSemester)
						{
							currentSemester = course.Semester;
							Console.WriteLine($"\n--- {currentSemester} ---");
						}
						Console.WriteLine($"  {course}");
					}
				}
				else
				{
					Console.WriteLine("No courses available.");
				}

				Console.WriteLine("\nEnter Course ID to open, F to filter by semester, or P to go back:");
				var courseChoice = Console.ReadLine();

				if (courseChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitTeacherCourseSelection = true;
				}
				else if (courseChoice.Equals("F", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewCoursesBySemester();
				}
				else if (int.TryParse(courseChoice, out int courseId))
				{
					var selectedCourse = CourseServiceProxy.Current.Courses
						.FirstOrDefault(c => c.Id == courseId);

					if (selectedCourse != null)
					{
						ShowCourseMenu(selectedCourse);
					}
					else
					{
						Console.WriteLine("Invalid Course ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Course ID, F to filter, or P to go back.");
				}
			}
		}

		private static void ViewCoursesBySemester()
		{
			bool exitSemesterView = false;
			while (!exitSemesterView)
			{
				// Get unique semesters
				var semesters = CourseServiceProxy.Current.Courses
					.Select(c => c.Semester)
					.Distinct()
					.OrderBy(s => s)
					.ToList();

				if (!semesters.Any())
				{
					Console.WriteLine("No courses available.");
					return;
				}

				Console.WriteLine("\nAvailable Semesters:");
				for (int i = 0; i < semesters.Count; i++)
				{
					var semesterCourseCount = CourseServiceProxy.Current.Courses
						.Count(c => c.Semester == semesters[i]);
					Console.WriteLine($"{i + 1}. {semesters[i]} ({semesterCourseCount} course(s))");
				}
				Console.WriteLine("P. Go Back");

				Console.WriteLine("\nSelect semester number:");
				var semesterChoice = Console.ReadLine();

				if (semesterChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitSemesterView = true;
				}
				else if (int.TryParse(semesterChoice, out int semesterIndex)
						 && semesterIndex > 0 && semesterIndex <= semesters.Count)
				{
					var selectedSemester = semesters[semesterIndex - 1];
					ViewCoursesInSemester(selectedSemester);
				}
				else
				{
					Console.WriteLine("Invalid selection. Please try again.");
				}
			}
		}

		private static void ViewCoursesInSemester(string semester)
		{
			bool exitSemesterCourseView = false;
			while (!exitSemesterCourseView)
			{
				Console.WriteLine($"\n{semester} Courses:");

				var semesterCourses = CourseServiceProxy.Current.Courses
					.Where(c => c.Semester == semester)
					.OrderBy(c => c.Code)
					.ThenBy(c => c.Section)
					.ToList();

				if (semesterCourses.Any())
				{
					foreach (var course in semesterCourses)
					{
						Console.WriteLine($"  {course}");
					}
				}
				else
				{
					Console.WriteLine("No courses for this semester.");
				}

				Console.WriteLine("\nEnter Course ID to open (or P to go back):");
				var courseChoice = Console.ReadLine();

				if (courseChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitSemesterCourseView = true;
				}
				else if (int.TryParse(courseChoice, out int courseId))
				{
					var selectedCourse = semesterCourses.FirstOrDefault(c => c.Id == courseId);

					if (selectedCourse != null)
					{
						ShowCourseMenu(selectedCourse);
					}
					else
					{
						Console.WriteLine("Invalid Course ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Course ID or P to go back.");
				}
			}
		}

		private static void ShowCourseMenu(Course selectedCourse)
		{
			bool exitTeacherCourseMenu = false;
			while (!exitTeacherCourseMenu)
			{
				Console.WriteLine($"\n{selectedCourse.Name} [{selectedCourse.Code}] Menu:");
				Console.WriteLine("E. Enroll Student");
				Console.WriteLine("V. View Enrolled Students");
				Console.WriteLine("U. Unenroll Student");
				Console.WriteLine("A. Manage Assignments");
				Console.WriteLine("M. Manage Modules");
				Console.WriteLine("G. Grade Submissions");
				Console.WriteLine("D. Update Course Description");
				Console.WriteLine("R. Manage Assignment Groups");
				Console.WriteLine("F. View Student Final Grades");
				Console.WriteLine("P. Exit");
				var teacherCourseMenuChoice = Console.ReadLine();

				if (teacherCourseMenuChoice.Equals("E", StringComparison.InvariantCultureIgnoreCase))
				{
					EnrollStudent(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("V", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewEnrolledStudents(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("U", StringComparison.InvariantCultureIgnoreCase))
				{
					UnenrollStudent(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("A", StringComparison.InvariantCultureIgnoreCase))
				{
					ManageAssignments(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("M", StringComparison.InvariantCultureIgnoreCase))
				{
					ManageModules(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("G", StringComparison.InvariantCultureIgnoreCase))
				{
					GradeSubmissions(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("D", StringComparison.InvariantCultureIgnoreCase))
				{
					UpdateCourseDescription(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("R", StringComparison.InvariantCultureIgnoreCase)) 
				{
					ManageAGroups(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("F", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewStudentFinalGrades(selectedCourse);
				}
				else if (teacherCourseMenuChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitTeacherCourseMenu = true;
				}
			}
		}

		private static void EnrollStudent(Course selectedCourse)
		{
			bool exitEnrollMenu = false;
			while (!exitEnrollMenu)
			{
				Console.WriteLine($"\nEnroll Student in {selectedCourse.Name} [{selectedCourse.Code}]:");
				Console.WriteLine("1. Add Existing Student");
				Console.WriteLine("2. Add New Student");
				Console.WriteLine("P. Exit");
				var enrollChoice = Console.ReadLine();

				if (enrollChoice.Equals("1"))
				{
					Console.WriteLine("\nAvailable Students:");
					var availableStudents = StudentServiceProxy.Current.Students
						.Where(s => !selectedCourse.Students.Any(cs => cs.Id == s.Id))
						.ToList();

					if (availableStudents.Any())
					{
						availableStudents.ForEach(Console.WriteLine);
						Console.WriteLine("Enter Student ID to enroll (or P to go back):");

						var studentChoice = Console.ReadLine();

						if (studentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
						{
							// Do nothing
						}
						else if (int.TryParse(studentChoice, out int studentId))
						{
							var selectedStudentToEnroll = StudentServiceProxy.Current.Students
								.FirstOrDefault(s => s.Id == studentId);

							if (selectedStudentToEnroll != null && !selectedCourse.Students.Any(cs => cs.Id == selectedStudentToEnroll.Id))
							{
								selectedCourse.Students.Add(selectedStudentToEnroll);
								Console.WriteLine($"Successfully enrolled {selectedStudentToEnroll.Name} in {selectedCourse.Name}!");
							}
							else
							{
								Console.WriteLine("Invalid Student ID or student is already enrolled.");
							}
						}
						else
						{
							Console.WriteLine("Invalid input. Please enter a valid Student ID or P to go back.");
						}
					}
					else
					{
						Console.WriteLine("All students are already enrolled in this course.");
					}
				}
				else if (enrollChoice.Equals("2"))
				{
					Console.WriteLine("Name:");
					var newName = Console.ReadLine();
					Console.WriteLine("Code:");
					var newCode = Console.ReadLine();

					Console.WriteLine("\nClassification:");
					Console.WriteLine("F. Freshman");
					Console.WriteLine("S. Sophomore");
					Console.WriteLine("J. Junior");
					Console.WriteLine("R. Senior");
					Console.WriteLine("U. Unknown");
					var classificationChoice = Console.ReadLine();

					string newClassification = "Unknown";
					if (classificationChoice.Equals("F", StringComparison.InvariantCultureIgnoreCase))
					{
						newClassification = "Freshman";
					}
					else if (classificationChoice.Equals("S", StringComparison.InvariantCultureIgnoreCase))
					{
						newClassification = "Sophomore";
					}
					else if (classificationChoice.Equals("J", StringComparison.InvariantCultureIgnoreCase))
					{
						newClassification = "Junior";
					}
					else if (classificationChoice.Equals("R", StringComparison.InvariantCultureIgnoreCase))
					{
						newClassification = "Senior";
					}
					else if (classificationChoice.Equals("U", StringComparison.InvariantCultureIgnoreCase))
					{
						newClassification = "Unknown";
					}
					else
					{
						Console.WriteLine("Invalid choice. Setting classification to 'Unknown'.");
						newClassification = "Unknown";
					}

					var newStudent = new Student
					{
						Name = newName,
						Code = newCode,
						Classification = newClassification
					};

					StudentServiceProxy.Current.Add(newStudent);
					selectedCourse.Students.Add(newStudent);

					Console.WriteLine($"Successfully created and enrolled {newStudent.Name} in {selectedCourse.Name}!");
				}
				else if (enrollChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitEnrollMenu = true;
				}
			}
		}

		private static void ViewEnrolledStudents(Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} - Enrolled Students:");
			if (selectedCourse.Students.Any())
			{
				selectedCourse.Students.ForEach(Console.WriteLine);
			}
			else
			{
				Console.WriteLine("No students enrolled yet.");
			}
		}

		private static void UnenrollStudent(Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} - Enrolled Students:");
			if (selectedCourse.Students.Any())
			{
				selectedCourse.Students.ForEach(Console.WriteLine);
				Console.WriteLine("Enter Student ID to unenroll (or P to go back):");

				var unenrollChoice = Console.ReadLine();

				if (unenrollChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					// Do nothing
				}
				else if (int.TryParse(unenrollChoice, out int studentId))
				{
					var studentToUnenroll = selectedCourse.Students
						.FirstOrDefault(s => s.Id == studentId);

					if (studentToUnenroll != null)
					{
						Console.WriteLine($"Are you sure you want to unenroll {studentToUnenroll.Name}? (Y/N):");
						var confirm = Console.ReadLine();

						if (confirm.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
						{
							selectedCourse.Students.Remove(studentToUnenroll);
							Console.WriteLine($"Successfully unenrolled {studentToUnenroll.Name} from {selectedCourse.Name}.");
						}
						else
						{
							Console.WriteLine("Unenrollment cancelled.");
						}
					}
					else
					{
						Console.WriteLine("Invalid Student ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Student ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No students enrolled in this course.");
			}
		}

		private static void ManageAssignments(Course selectedCourse)
		{
			bool exitAssignmentMenu = false;
			while (!exitAssignmentMenu)
			{
				Console.WriteLine($"\n{selectedCourse.Name} - Assignments:");
				if (selectedCourse.Assignments.Any())
				{
					foreach (var assignment in selectedCourse.Assignments)
					{
						Console.WriteLine($"  [{assignment.Id}] {assignment.Name} - {assignment.Description} (Points: {assignment.AvailablePoints}, Due: {assignment.DueDate})");
					}
				}
				else
				{
					Console.WriteLine("No assignments yet.");
				}

				Console.WriteLine("\nC. Create New Assignment");
				Console.WriteLine("E. Edit Assignment");
				Console.WriteLine("D. Delete Assignment");
				Console.WriteLine("P. Exit");
				var assignmentChoice = Console.ReadLine();

				if (assignmentChoice.Equals("C", StringComparison.InvariantCultureIgnoreCase))
				{
					CreateAssignment(selectedCourse);
				}
				else if (assignmentChoice.Equals("E", StringComparison.InvariantCultureIgnoreCase))
				{
					EditAssignment(selectedCourse);
				}
				else if (assignmentChoice.Equals("D", StringComparison.InvariantCultureIgnoreCase))
				{
					DeleteAssignment(selectedCourse);
				}
				else if (assignmentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitAssignmentMenu = true;
				}
			}
		}

		private static void CreateAssignment(Course selectedCourse)
		{
			Console.WriteLine("Assignment Name:");
			var assignmentName = Console.ReadLine();
			Console.WriteLine("Description:");
			var assignmentDescription = Console.ReadLine();
			Console.WriteLine("Available Points:");
			var pointsInput = Console.ReadLine();
			Console.WriteLine("Due Date (YYYY-MM-DD):");
			var dueDate = Console.ReadLine();

			if (int.TryParse(pointsInput, out int points))
			{
				var newAssignment = new Assignment
				{
					Name = assignmentName,
					Description = assignmentDescription,
					AvailablePoints = points,
					DueDate = dueDate
				};

				AssignmentServiceProxy.Current.Add(newAssignment);
				selectedCourse.Assignments.Add(newAssignment);

				Console.WriteLine($"Successfully created assignment: {newAssignment.Name}");
			}
			else
			{
				Console.WriteLine("Invalid points value. Assignment not created.");
			}
		}

		private static void EditAssignment(Course selectedCourse)
		{
			if (selectedCourse.Assignments.Any())
			{
				Console.WriteLine("Enter Assignment ID to edit (or P to go back):");
				var editChoice = Console.ReadLine();

				if (editChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else if (int.TryParse(editChoice, out int assignmentId))
				{
					var assignmentToEdit = selectedCourse.Assignments
						.FirstOrDefault(a => a.Id == assignmentId);

					if (assignmentToEdit != null)
					{
						Console.WriteLine($"\nEditing: {assignmentToEdit.Name}");
						Console.WriteLine("Leave blank to keep current value.\n");

						Console.WriteLine($"Current Name: {assignmentToEdit.Name}");
						Console.WriteLine("New Name:");
						var newName = Console.ReadLine();
						if (!string.IsNullOrWhiteSpace(newName))
						{
							assignmentToEdit.Name = newName;
						}

						Console.WriteLine($"Current Description: {assignmentToEdit.Description}");
						Console.WriteLine("New Description:");
						var newDescription = Console.ReadLine();
						if (!string.IsNullOrWhiteSpace(newDescription))
						{
							assignmentToEdit.Description = newDescription;
						}

						Console.WriteLine($"Current Available Points: {assignmentToEdit.AvailablePoints}");
						Console.WriteLine("New Available Points:");
						var newPointsInput = Console.ReadLine();
						if (!string.IsNullOrWhiteSpace(newPointsInput) && int.TryParse(newPointsInput, out int newPoints))
						{
							assignmentToEdit.AvailablePoints = newPoints;
						}

						Console.WriteLine($"Current Due Date: {assignmentToEdit.DueDate}");
						Console.WriteLine("New Due Date (YYYY-MM-DD):");
						var newDueDate = Console.ReadLine();
						if (!string.IsNullOrWhiteSpace(newDueDate))
						{
							assignmentToEdit.DueDate = newDueDate;
						}

						Console.WriteLine($"\nSuccessfully updated assignment: {assignmentToEdit.Name}");
					}
					else
					{
						Console.WriteLine("Invalid Assignment ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Assignment ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No assignments to edit.");
			}
		}

		private static void DeleteAssignment(Course selectedCourse)
		{
			if (selectedCourse.Assignments.Any())
			{
				Console.WriteLine("Enter Assignment ID to delete (or P to go back):");
				var deleteChoice = Console.ReadLine();

				if (deleteChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else if (int.TryParse(deleteChoice, out int assignmentId))
				{
					var assignmentToDelete = selectedCourse.Assignments
						.FirstOrDefault(a => a.Id == assignmentId);

					if (assignmentToDelete != null)
					{
						var submissionCount = assignmentToDelete.Submissions.Count;
						Console.WriteLine($"Are you sure you want to delete '{assignmentToDelete.Name}'?");
						Console.WriteLine($"This will also delete {submissionCount} submission(s). (Y/N):");
						var confirm = Console.ReadLine();

						if (confirm.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
						{
							foreach (var submission in assignmentToDelete.Submissions.ToList())
							{
								SubmitServiceProxy.Current.Submissions.Remove(submission);
							}

							selectedCourse.Assignments.Remove(assignmentToDelete);
							AssignmentServiceProxy.Current.Assignments.Remove(assignmentToDelete);

							Console.WriteLine($"Successfully deleted assignment '{assignmentToDelete.Name}' and {submissionCount} submission(s).");
						}
						else
						{
							Console.WriteLine("Deletion cancelled.");
						}
					}
					else
					{
						Console.WriteLine("Invalid Assignment ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Assignment ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No assignments to delete.");
			}
		}

		private static void ManageModules(Course selectedCourse)
		{
			bool exitModuleMenu = false;
			while (!exitModuleMenu)
			{
				Console.WriteLine($"\n{selectedCourse.Name} - Modules:");
				if (selectedCourse.Modules.Any())
				{
					foreach (var module in selectedCourse.Modules)
					{
						Console.WriteLine($"  [Module {module.Id}] {module.Content.Count} content item(s)");
					}
				}
				else
				{
					Console.WriteLine("No modules yet.");
				}

				Console.WriteLine("\nC. Create New Module");
				Console.WriteLine("A. Add Content to Module");
				Console.WriteLine("E. Edit Content in Module");
				Console.WriteLine("D. Delete Content from Module");
				Console.WriteLine("P. Exit");
				var moduleChoice = Console.ReadLine();

				if (moduleChoice.Equals("C", StringComparison.InvariantCultureIgnoreCase))
				{
					CreateModule(selectedCourse);
				}
				else if (moduleChoice.Equals("A", StringComparison.InvariantCultureIgnoreCase))
				{
					AddContentToModule(selectedCourse);
				}
				else if (moduleChoice.Equals("E", StringComparison.InvariantCultureIgnoreCase))
				{
					EditContentInModule(selectedCourse);
				}
				else if (moduleChoice.Equals("D", StringComparison.InvariantCultureIgnoreCase))
				{
					DeleteContentFromModule(selectedCourse);
				}
				else if (moduleChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitModuleMenu = true;
				}
			}
		}

		private static void CreateModule(Course selectedCourse)
		{
			var newModule = new Module();
			ModuleServiceProxy.Current.Add(newModule);
			selectedCourse.Modules.Add(newModule);
			Console.WriteLine($"Successfully created Module {newModule.Id} (empty module - add content next)");
		}

		private static void AddContentToModule(Course selectedCourse)
		{
			if (!selectedCourse.Modules.Any())
			{
				Console.WriteLine("No modules available. Create a module first.");
				return;
			}

			Console.WriteLine("Enter Module ID to add content (or P to go back):");
			var moduleIdChoice = Console.ReadLine();

			if (moduleIdChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			else if (int.TryParse(moduleIdChoice, out int moduleId))
			{
				var selectedModule = selectedCourse.Modules.FirstOrDefault(m => m.Id == moduleId);

				if (selectedModule != null)
				{
					Console.WriteLine($"\nModule {selectedModule.Id} - Current Content:");
					if (selectedModule.Content.Any())
					{
						for (int i = 0; i < selectedModule.Content.Count; i++)
						{
							Console.WriteLine($"  {i + 1}. {selectedModule.Content[i]}");
						}
					}
					else
					{
						Console.WriteLine("  (No content yet)");
					}

					Console.WriteLine("\nSelect content type to add:");
					Console.WriteLine("1. Page (text content)");
					Console.WriteLine("2. File");
					Console.WriteLine("3. Assignment (embed existing assignment)");
					Console.WriteLine("P. Cancel");
					var contentTypeChoice = Console.ReadLine();

					if (contentTypeChoice.Equals("1"))
					{
						Console.WriteLine("Page Name:");
						var pageName = Console.ReadLine();
						Console.WriteLine("Page Content:");
						var pageContent = Console.ReadLine();

						if (!string.IsNullOrWhiteSpace(pageName) && !string.IsNullOrWhiteSpace(pageContent))
						{
							var newPage = new Page
							{
								Id = ModuleServiceProxy.Current.NextContentKey,
								Name = pageName,
								Content = pageContent
							};
							selectedModule.Content.Add(newPage);
							Console.WriteLine($"Successfully added page '{pageName}' to Module {selectedModule.Id}");
						}
						else
						{
							Console.WriteLine("Name and content cannot be empty. Nothing added.");
						}
					}
					else if (contentTypeChoice.Equals("2"))
					{
						Console.WriteLine("File Name:");
						var fileName = Console.ReadLine();
						Console.WriteLine("File Path:");
						var filePath = Console.ReadLine();

						if (!string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(filePath))
						{
							var newFile = new FilePlus
							{
								Id = ModuleServiceProxy.Current.NextContentKey,
								Name = fileName,
								FilePath = filePath
							};
							selectedModule.Content.Add(newFile);
							Console.WriteLine($"Successfully added file '{fileName}' to Module {selectedModule.Id}");
						}
						else
						{
							Console.WriteLine("Name and path cannot be empty. Nothing added.");
						}
					}
					else if (contentTypeChoice.Equals("3"))
					{
						if (selectedCourse.Assignments.Any())
						{
							Console.WriteLine("\nAvailable Assignments:");
							selectedCourse.Assignments.ForEach(Console.WriteLine);
							Console.WriteLine("Enter Assignment ID to embed (or P to cancel):");

							var assignmentChoice = Console.ReadLine();

							if (!assignmentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase)
								&& int.TryParse(assignmentChoice, out int assignmentId))
							{
								var assignmentToEmbed = selectedCourse.Assignments.FirstOrDefault(a => a.Id == assignmentId);

								if (assignmentToEmbed != null)
								{
									var newAssignPlus = new AssignPlus
									{
										Id = ModuleServiceProxy.Current.NextContentKey,
										Name = assignmentToEmbed.Name,
										Assignment = assignmentToEmbed
									};
									selectedModule.Content.Add(newAssignPlus);
									Console.WriteLine($"Successfully embedded assignment '{assignmentToEmbed.Name}' in Module {selectedModule.Id}");
								}
								else
								{
									Console.WriteLine("Invalid Assignment ID.");
								}
							}
						}
						else
						{
							Console.WriteLine("No assignments available to embed. Create an assignment first.");
						}
					}
				}
				else
				{
					Console.WriteLine("Invalid Module ID. Please try again.");
				}
			}
			else
			{
				Console.WriteLine("Invalid input. Please enter a valid Module ID or P to go back.");
			}
		}

		private static void EditContentInModule(Course selectedCourse)
		{
			if (!selectedCourse.Modules.Any())
			{
				Console.WriteLine("No modules available. Create a module first.");
				return;
			}

			Console.WriteLine("Enter Module ID to edit content (or P to go back):");
			var moduleIdChoice = Console.ReadLine();

			if (moduleIdChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			else if (int.TryParse(moduleIdChoice, out int moduleId))
			{
				var selectedModule = selectedCourse.Modules.FirstOrDefault(m => m.Id == moduleId);

				if (selectedModule != null)
				{
					if (selectedModule.Content.Any())
					{
						Console.WriteLine($"\nModule {selectedModule.Id} - Current Content:");
						for (int i = 0; i < selectedModule.Content.Count; i++)
						{
							Console.WriteLine($"  {i + 1}. {selectedModule.Content[i]}");
						}

						Console.WriteLine("\nEnter content number to edit (or P to go back):");
						var contentChoice = Console.ReadLine();

						if (!contentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase)
							&& int.TryParse(contentChoice, out int contentNumber)
							&& contentNumber > 0 && contentNumber <= selectedModule.Content.Count)
						{
							var contentIndex = contentNumber - 1;
							var contentToEdit = selectedModule.Content[contentIndex];

							Console.WriteLine($"\nEditing: {contentToEdit.Name}");

							if (contentToEdit is Page page)
							{
								Console.WriteLine($"Current Name: {page.Name}");
								Console.WriteLine("New Name (or press Enter to keep current):");
								var newName = Console.ReadLine();
								if (!string.IsNullOrWhiteSpace(newName))
								{
									page.Name = newName;
								}

								Console.WriteLine($"Current Content: {page.Content}");
								Console.WriteLine("New Content (or press Enter to keep current):");
								var newContent = Console.ReadLine();
								if (!string.IsNullOrWhiteSpace(newContent))
								{
									page.Content = newContent;
								}

								Console.WriteLine("Successfully updated page.");
							}
							else if (contentToEdit is FilePlus filePlus)
							{
								Console.WriteLine($"Current Name: {filePlus.Name}");
								Console.WriteLine("New Name (or press Enter to keep current):");
								var newName = Console.ReadLine();
								if (!string.IsNullOrWhiteSpace(newName))
								{
									filePlus.Name = newName;
								}

								Console.WriteLine($"Current Path: {filePlus.FilePath}");
								Console.WriteLine("New Path (or press Enter to keep current):");
								var newPath = Console.ReadLine();
								if (!string.IsNullOrWhiteSpace(newPath))
								{
									filePlus.FilePath = newPath;
								}

								Console.WriteLine("Successfully updated file.");
							}
							else if (contentToEdit is AssignPlus)
							{
								Console.WriteLine("Assignment items cannot be edited directly.");
								Console.WriteLine("Edit the assignment from the Manage Assignments menu instead.");
							}
						}
						else if (!contentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
						{
							Console.WriteLine("Invalid content number. Please try again.");
						}
					}
					else
					{
						Console.WriteLine("This module has no content to edit.");
					}
				}
				else
				{
					Console.WriteLine("Invalid Module ID. Please try again.");
				}
			}
			else
			{
				Console.WriteLine("Invalid input. Please enter a valid Module ID or P to go back.");
			}
		}

		private static void DeleteContentFromModule(Course selectedCourse)
		{
			if (!selectedCourse.Modules.Any())
			{
				Console.WriteLine("No modules available. Create a module first.");
				return;
			}

			Console.WriteLine("Enter Module ID to delete content (or P to go back):");
			var moduleIdChoice = Console.ReadLine();

			if (moduleIdChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			else if (int.TryParse(moduleIdChoice, out int moduleId))
			{
				var selectedModule = selectedCourse.Modules.FirstOrDefault(m => m.Id == moduleId);

				if (selectedModule != null)
				{
					if (selectedModule.Content.Any())
					{
						Console.WriteLine($"\nModule {selectedModule.Id} - Current Content:");
						for (int i = 0; i < selectedModule.Content.Count; i++)
						{
							Console.WriteLine($"  {i + 1}. {selectedModule.Content[i]}");
						}

						Console.WriteLine("\nEnter content number to delete (or P to go back):");
						var contentChoice = Console.ReadLine();

						if (!contentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase)
							&& int.TryParse(contentChoice, out int contentNumber)
							&& contentNumber > 0 && contentNumber <= selectedModule.Content.Count)
						{
							var contentIndex = contentNumber - 1;
							var deletedContent = selectedModule.Content[contentIndex];

							Console.WriteLine($"Are you sure you want to delete: '{deletedContent.Name}'? (Y/N):");
							var confirm = Console.ReadLine();

							if (confirm.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
							{
								selectedModule.Content.RemoveAt(contentIndex);
								Console.WriteLine($"Successfully deleted content from Module {selectedModule.Id}");
							}
							else
							{
								Console.WriteLine("Deletion cancelled.");
							}
						}
						else if (!contentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
						{
							Console.WriteLine("Invalid content number. Please try again.");
						}
					}
					else
					{
						Console.WriteLine("This module has no content to delete.");
					}
				}
				else
				{
					Console.WriteLine("Invalid Module ID. Please try again.");
				}
			}
			else
			{
				Console.WriteLine("Invalid input. Please enter a valid Module ID or P to go back.");
			}
		}

		private static void GradeSubmissions(Course selectedCourse)
		{
			bool exitGradeMenu = false;
			while (!exitGradeMenu)
			{
				Console.WriteLine($"\n{selectedCourse.Name} - Assignments:");
				if (selectedCourse.Assignments.Any())
				{
					foreach (var assignment in selectedCourse.Assignments)
					{
						var submissionCount = assignment.Submissions.Count;
						Console.WriteLine($"  [{assignment.Id}] {assignment.Name} - {submissionCount} submission(s)");
					}
					Console.WriteLine("Enter Assignment ID to view submissions (or P to go back):");

					var assignmentChoice = Console.ReadLine();

					if (assignmentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
					{
						exitGradeMenu = true;
					}
					else if (int.TryParse(assignmentChoice, out int assignmentId))
					{
						var selectedAssignment = selectedCourse.Assignments.FirstOrDefault(a => a.Id == assignmentId);

						if (selectedAssignment != null)
						{
							GradeAssignmentSubmissions(selectedAssignment);
						}
						else
						{
							Console.WriteLine("Invalid Assignment ID. Please try again.");
						}
					}
					else
					{
						Console.WriteLine("Invalid input. Please enter a valid Assignment ID or P to go back.");
					}
				}
				else
				{
					Console.WriteLine("No assignments available for this course.");
					exitGradeMenu = true;
				}
			}
		}

		private static void GradeAssignmentSubmissions(Assignment selectedAssignment)
		{
			bool exitSubmissionView = false;
			while (!exitSubmissionView)
			{
				Console.WriteLine($"\n{selectedAssignment.Name} - Submissions:");
				if (selectedAssignment.Submissions.Any())
				{
					foreach (var submission in selectedAssignment.Submissions)
					{
						var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == submission.StudentId);
						var studentName = student != null ? student.Name : "Unknown";
						var gradeStatus = submission.Grade.HasValue ? $"Grade: {submission.Grade}/{selectedAssignment.AvailablePoints}" : "Not Graded";
						Console.WriteLine($"  [{submission.Id}] {studentName} - Submitted: {submission.SubmissionDate} - {gradeStatus}");
					}
					Console.WriteLine("\nEnter Submission ID to review and grade (or P to go back):");

					var submissionChoice = Console.ReadLine();

					if (submissionChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
					{
						exitSubmissionView = true;
					}
					else if (int.TryParse(submissionChoice, out int submissionId))
					{
						var selectedSubmission = selectedAssignment.Submissions.FirstOrDefault(s => s.Id == submissionId);

						if (selectedSubmission != null)
						{
							var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == selectedSubmission.StudentId);
							var studentName = student != null ? student.Name : "Unknown";

							Console.WriteLine($"\n--- Submission Details ---");
							Console.WriteLine($"Student: {studentName}");
							Console.WriteLine($"Assignment: {selectedAssignment.Name}");
							Console.WriteLine($"Submitted: {selectedSubmission.SubmissionDate}");
							Console.WriteLine($"Content:\n{selectedSubmission.Content}");
							Console.WriteLine($"--- End of Content ---");
							Console.WriteLine($"\nCurrent Grade: {(selectedSubmission.Grade.HasValue ? $"{selectedSubmission.Grade}/{selectedAssignment.AvailablePoints}" : "Not Graded")}");
							Console.WriteLine($"Current Comment: {(string.IsNullOrWhiteSpace(selectedSubmission.Comment) ? "No comment" : selectedSubmission.Comment)}");
							Console.WriteLine($"Available Points: {selectedAssignment.AvailablePoints}");
							Console.WriteLine($"--- End of Submission ---\n");

							Console.WriteLine("How would you like to grade?");
							Console.WriteLine("1. Enter points (0-{0})", selectedAssignment.AvailablePoints);
							Console.WriteLine("2. Enter percentage (Max 100%)");  // UPDATED TEXT
							Console.WriteLine("P. Skip grading");
							var gradingMethod = Console.ReadLine();

							int? newGrade = null;

							if (gradingMethod.Equals("1"))
							{
								Console.WriteLine($"Enter points (0-{selectedAssignment.AvailablePoints}):");
								var pointsInput = Console.ReadLine();

								if (!string.IsNullOrWhiteSpace(pointsInput) && int.TryParse(pointsInput, out int points))
								{
									if (points >= 0 && points <= selectedAssignment.AvailablePoints)
									{
										newGrade = points;
									}
									else
									{
										Console.WriteLine($"Invalid points. Must be between 0 and {selectedAssignment.AvailablePoints}.");
									}
								}
							}
							else if (gradingMethod.Equals("2"))
							{
								Console.WriteLine("Enter percentage (Max 100%):");  // UPDATED TEXT
								var percentageInput = Console.ReadLine();

								// REMOVE % SYMBOL IF PRESENT
								if (!string.IsNullOrWhiteSpace(percentageInput))
								{
									percentageInput = percentageInput.Trim().TrimEnd('%');

									if (double.TryParse(percentageInput, out double percentage))
									{
										if (percentage >= 0 && percentage <= 100)
										{
											// Convert percentage to points
											newGrade = (int)Math.Round((percentage / 100.0) * selectedAssignment.AvailablePoints);
											Console.WriteLine($"Percentage {percentage}% = {newGrade} points out of {selectedAssignment.AvailablePoints}");
										}
										else
										{
											Console.WriteLine("Invalid percentage. Must be between 0 and 100.");
										}
									}
									else
									{
										Console.WriteLine("Invalid percentage format.");
									}
								}
							}
							else if (gradingMethod.Equals("P", StringComparison.InvariantCultureIgnoreCase))
							{
								Console.WriteLine("Grading skipped.");
								continue;
							}

							// If a grade was entered, ask for comment
							if (newGrade.HasValue)
							{
								selectedSubmission.Grade = newGrade.Value;

								Console.WriteLine("\nEnter feedback comment (or press Enter to skip):");
								var comment = Console.ReadLine();

								if (!string.IsNullOrWhiteSpace(comment))
								{
									selectedSubmission.Comment = comment;
								}

								Console.WriteLine($"\nSuccessfully graded submission: {selectedSubmission.Grade}/{selectedAssignment.AvailablePoints}");
								if (!string.IsNullOrWhiteSpace(selectedSubmission.Comment))
								{
									Console.WriteLine($"Comment: {selectedSubmission.Comment}");
								}
							}
						}
						else
						{
							Console.WriteLine("Invalid Submission ID. Please try again.");
						}
					}
					else
					{
						Console.WriteLine("Invalid input. Please enter a valid Submission ID or P to go back.");
					}
				}
				else
				{
					Console.WriteLine("No submissions yet for this assignment.");
					exitSubmissionView = true;
				}
			}
		}

		private static void UpdateCourseDescription(Course selectedCourse)
		{
			Console.WriteLine($"\nCurrent Description: {selectedCourse.Description}");
			Console.WriteLine("Enter new description (or press Enter to keep current):");
			var newDescription = Console.ReadLine();

			if (!string.IsNullOrWhiteSpace(newDescription))
			{
				selectedCourse.Description = newDescription;
				Console.WriteLine($"Successfully updated course description for {selectedCourse.Name}.");
			}
			else
			{
				Console.WriteLine("Description unchanged.");
			}
		}

		private static void DeleteCourse()
		{
			if (CourseServiceProxy.Current.Courses.Any())
			{
				Console.WriteLine("\nAll Courses:");
				CourseServiceProxy.Current.Courses.ForEach(Console.WriteLine);
				Console.WriteLine("Enter Course ID to delete (or P to go back):");

				var deleteChoice = Console.ReadLine();

				if (deleteChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else if (int.TryParse(deleteChoice, out int courseId))
				{
					var courseToDelete = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);

					if (courseToDelete != null)
					{
						var enrolledCount = courseToDelete.Students.Count;
						var assignmentCount = courseToDelete.Assignments.Count;
						var moduleCount = courseToDelete.Modules.Count;

						Console.WriteLine($"Are you sure you want to delete '{courseToDelete.Name} [{courseToDelete.Code}]'?");
						Console.WriteLine($"This will delete {assignmentCount} assignment(s) and {moduleCount} module(s).");
						Console.WriteLine($"{enrolledCount} student(s) will be unenrolled but will remain in the system. (Y/N):");
						var confirm = Console.ReadLine();

						if (confirm.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
						{
							courseToDelete.Students.Clear();

							foreach (var assignment in courseToDelete.Assignments)
							{
								foreach (var submission in assignment.Submissions.ToList())
								{
									SubmitServiceProxy.Current.Submissions.Remove(submission);
								}
								AssignmentServiceProxy.Current.Assignments.Remove(assignment);
							}

							foreach (var module in courseToDelete.Modules.ToList())
							{
								ModuleServiceProxy.Current.Modules.Remove(module);
							}

							CourseServiceProxy.Current.Courses.Remove(courseToDelete);

							Console.WriteLine($"Successfully deleted course '{courseToDelete.Name}'.");
							Console.WriteLine($"Unenrolled {enrolledCount} student(s), deleted {assignmentCount} assignment(s) and {moduleCount} module(s).");
						}
						else
						{
							Console.WriteLine("Deletion cancelled.");
						}
					}
					else
					{
						Console.WriteLine("Invalid Course ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Course ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No courses available to delete.");
			}
		}
		private static void CopyCourse()
		{
			if (CourseServiceProxy.Current.Courses.Any())
			{
				Console.WriteLine("\nAll Courses:");
				CourseServiceProxy.Current.Courses.ForEach(Console.WriteLine);
				Console.WriteLine("Enter Course ID to copy (or P to go back):");

				var copyChoice = Console.ReadLine();

				if (copyChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else if (int.TryParse(copyChoice, out int courseId))
				{
					var courseToCopy = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);

					if (courseToCopy != null)
					{
						Console.WriteLine($"\nCopying course: {courseToCopy.Name}");
						Console.WriteLine("Enter new course code:");
						var newCode = Console.ReadLine();
						Console.WriteLine("Enter new course name (or press Enter to use '{0} - Copy'):", courseToCopy.Name);
						var newName = Console.ReadLine();
						if (string.IsNullOrWhiteSpace(newName))
						{
							newName = $"{courseToCopy.Name} - Copy";
						}
						Console.WriteLine("Enter semester for copied course (or press Enter to keep '{0}'):", courseToCopy.Semester);
						var newSemester = Console.ReadLine();
						if (string.IsNullOrWhiteSpace(newSemester))
						{
							newSemester = courseToCopy.Semester;
						}
						Console.WriteLine("Enter section for copied course (or press Enter to keep '{0}'):", courseToCopy.Section);
						var newSection = Console.ReadLine();
						if (string.IsNullOrWhiteSpace(newSection))
						{
							newSection = courseToCopy.Section;
						}

						var copiedCourse = DeepCopyCourse(courseToCopy, newCode, newName, newSemester, newSection);

						// Add to master list
						CourseServiceProxy.Current.Add(copiedCourse);

						Console.WriteLine($"Successfully copied course: {copiedCourse.Name} [{copiedCourse.Code}]");
						Console.WriteLine($"Copied {copiedCourse.Modules.Count} module(s), {copiedCourse.Assignments.Count} assignment(s), and {copiedCourse.AGroups.Count} assignment group(s).");
						Console.WriteLine("Note: Student roster and submissions were NOT copied.");
					}
					else
					{
						Console.WriteLine("Invalid Course ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Course ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No courses available to copy.");
			}
		}

		private static Course DeepCopyCourse(Course original, string newCode, string newName, string newSemester, string newSection)
		{
			var copiedCourse = new Course
			{
				Name = newName,
				Code = newCode,
				Description = original.Description,
				Semester = newSemester,
				Section = newSection
			};

			// Deep copy modules and their content
			foreach (var originalModule in original.Modules)
			{
				var copiedModule = new Module();
				ModuleServiceProxy.Current.Add(copiedModule);

				// Deep copy content items
				foreach (var originalContent in originalModule.Content)
				{
					if (originalContent is Page originalPage)
					{
						var copiedPage = new Page
						{
							Id = ModuleServiceProxy.Current.NextContentKey,
							Name = originalPage.Name,
							Content = originalPage.Content
						};
						copiedModule.Content.Add(copiedPage);
					}
					else if (originalContent is FilePlus originalFile)
					{
						var copiedFile = new FilePlus
						{
							Id = ModuleServiceProxy.Current.NextContentKey,
							Name = originalFile.Name,
							FilePath = originalFile.FilePath
						};
						copiedModule.Content.Add(copiedFile);
					}
					else if (originalContent is AssignPlus originalAssignPlus)
					{
						// Note: We'll create a reference to the copied assignment after assignments are copied
						// For now, skip AssignPlus - we'll handle this after copying assignments
					}
				}

				copiedCourse.Modules.Add(copiedModule);
			}

			// Deep copy assignments (without submissions)
			var assignmentMapping = new Dictionary<int, Assignment>(); // Map original ID to copied assignment

			foreach (var originalAssignment in original.Assignments)
			{
				var copiedAssignment = new Assignment
				{
					Name = originalAssignment.Name,
					Description = originalAssignment.Description,
					AvailablePoints = originalAssignment.AvailablePoints,
					DueDate = originalAssignment.DueDate
					// Submissions list is initialized empty - NOT copied
				};

				AssignmentServiceProxy.Current.Add(copiedAssignment);
				copiedCourse.Assignments.Add(copiedAssignment);
				assignmentMapping[originalAssignment.Id] = copiedAssignment;
			}

			// Now add embedded assignments (AssignPlus) to modules with references to copied assignments
			for (int i = 0; i < original.Modules.Count; i++)
			{
				var originalModule = original.Modules[i];
				var copiedModule = copiedCourse.Modules[i];

				foreach (var originalContent in originalModule.Content)
				{
					if (originalContent is AssignPlus originalAssignPlus)
					{
						if (assignmentMapping.ContainsKey(originalAssignPlus.Assignment.Id))
						{
							var copiedAssignPlus = new AssignPlus
							{
								Id = ModuleServiceProxy.Current.NextContentKey,
								Name = originalAssignPlus.Name,
								Assignment = assignmentMapping[originalAssignPlus.Assignment.Id]
							};
							copiedModule.Content.Add(copiedAssignPlus);
						}
					}
				}
			}

			// Deep copy assignment groups
			foreach (var originalAGroup in original.AGroups)
			{
				var copiedAGroup = new AGroup
				{
					Name = originalAGroup.Name,
					Weight = originalAGroup.Weight
				};

				// Add copied assignments to the group (using the mapping)
				foreach (var originalAssignment in originalAGroup.Assignments)
				{
					if (assignmentMapping.ContainsKey(originalAssignment.Id))
					{
						copiedAGroup.Assignments.Add(assignmentMapping[originalAssignment.Id]);
					}
				}

				AGroupServiceProxy.Current.Add(copiedAGroup);
				copiedCourse.AGroups.Add(copiedAGroup);
			}

			// Students list is initialized empty - NOT copied
			// Submissions are not copied (they're part of assignments, and we created new empty assignments)

			return copiedCourse;
		}
		private static void ManageAGroups(Course selectedCourse)
		{
			bool exitAGroupMenu = false;
			while (!exitAGroupMenu)
			{
				Console.WriteLine($"\n{selectedCourse.Name} - Assignment Groups Menu:");
				Console.WriteLine("C. Create New Assignment Group");
				Console.WriteLine("L. List Assignment Groups");
				Console.WriteLine("E. Edit Assignment Group");
				Console.WriteLine("D. Delete Assignment Group");
				Console.WriteLine("A. Add Assignment to Group");
				Console.WriteLine("P. Exit");
				var agroupChoice = Console.ReadLine();

				if (agroupChoice.Equals("C", StringComparison.InvariantCultureIgnoreCase))
				{
					CreateAGroup(selectedCourse);
				}
				else if (agroupChoice.Equals("L", StringComparison.InvariantCultureIgnoreCase))
				{
					ListAGroups(selectedCourse);
				}
				else if (agroupChoice.Equals("E", StringComparison.InvariantCultureIgnoreCase))
				{
					EditAGroup(selectedCourse);
				}
				else if (agroupChoice.Equals("D", StringComparison.InvariantCultureIgnoreCase))
				{
					DeleteAGroup(selectedCourse);
				}
				else if (agroupChoice.Equals("A", StringComparison.InvariantCultureIgnoreCase))
				{
					AddAssignmentToGroup(selectedCourse);
				}
				else if (agroupChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitAGroupMenu = true;
				}
			}
		}

		private static void CreateAGroup(Course selectedCourse)
		{
			Console.WriteLine("Assignment Group Name:");
			var agroupName = Console.ReadLine();

			Console.WriteLine("Weight (as percentage, e.g., 25 for 25%):");
			var weightInput = Console.ReadLine();

			if (!string.IsNullOrWhiteSpace(agroupName) && double.TryParse(weightInput, out double weightPercent))
			{
				var newAGroup = new AGroup
				{
					Name = agroupName,
					Weight = weightPercent / 100.0  // Convert percentage to decimal
				};

				AGroupServiceProxy.Current.Add(newAGroup);
				selectedCourse.AGroups.Add(newAGroup);

				Console.WriteLine($"Successfully created assignment group: {newAGroup.Name} with weight {weightPercent}%");
			}
			else
			{
				Console.WriteLine("Invalid input. Assignment group not created.");
			}
		}

		private static void ListAGroups(Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} - Assignment Groups:");
			if (selectedCourse.AGroups.Any())
			{
				foreach (var agroup in selectedCourse.AGroups)
				{
					Console.WriteLine($"  [{agroup.Id}] {agroup.Name} - Weight: {agroup.Weight * 100}% - {agroup.Assignments.Count} assignment(s)");
					if (agroup.Assignments.Any())
					{
						Console.WriteLine($"    Assignments in this group:");
						foreach (var assignment in agroup.Assignments)
						{
							Console.WriteLine($"      - {assignment.Name}");
						}
					}
					else
					{
						Console.WriteLine($"    (No assignments in this group)");
					}
				}
			}
			else
			{
				Console.WriteLine("No assignment groups yet.");
			}
		}

		private static void EditAGroup(Course selectedCourse)
		{
			if (selectedCourse.AGroups.Any())
			{
				Console.WriteLine("Enter Assignment Group ID to edit (or P to go back):");
				var editChoice = Console.ReadLine();

				if (editChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else if (int.TryParse(editChoice, out int agroupId))
				{
					var agroupToEdit = selectedCourse.AGroups.FirstOrDefault(g => g.Id == agroupId);

					if (agroupToEdit != null)
					{
						Console.WriteLine($"\nEditing: {agroupToEdit.Name}");
						Console.WriteLine("Leave blank to keep current value.\n");

						Console.WriteLine($"Current Name: {agroupToEdit.Name}");
						Console.WriteLine("New Name:");
						var newName = Console.ReadLine();
						if (!string.IsNullOrWhiteSpace(newName))
						{
							agroupToEdit.Name = newName;
						}

						Console.WriteLine($"Current Weight: {agroupToEdit.Weight * 100}%");
						Console.WriteLine("New Weight (as percentage, e.g., 25 for 25%):");
						var newWeightInput = Console.ReadLine();
						if (!string.IsNullOrWhiteSpace(newWeightInput) && double.TryParse(newWeightInput, out double newWeightPercent))
						{
							agroupToEdit.Weight = newWeightPercent / 100.0;
						}

						Console.WriteLine($"\nSuccessfully updated assignment group: {agroupToEdit.Name}");
					}
					else
					{
						Console.WriteLine("Invalid Assignment Group ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Assignment Group ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No assignment groups to edit.");
			}
		}

		private static void DeleteAGroup(Course selectedCourse)
		{
			if (selectedCourse.AGroups.Any())
			{
				Console.WriteLine("Enter Assignment Group ID to delete (or P to go back):");
				var deleteChoice = Console.ReadLine();

				if (deleteChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else if (int.TryParse(deleteChoice, out int agroupId))
				{
					var agroupToDelete = selectedCourse.AGroups.FirstOrDefault(g => g.Id == agroupId);

					if (agroupToDelete != null)
					{
						Console.WriteLine($"Are you sure you want to delete '{agroupToDelete.Name}'?");
						Console.WriteLine($"This will remove {agroupToDelete.Assignments.Count} assignment(s) from the group (assignments will remain in the course). (Y/N):");
						var confirm = Console.ReadLine();

						if (confirm.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
						{
							selectedCourse.AGroups.Remove(agroupToDelete);
							AGroupServiceProxy.Current.AGroups.Remove(agroupToDelete);

							Console.WriteLine($"Successfully deleted assignment group '{agroupToDelete.Name}'.");
						}
						else
						{
							Console.WriteLine("Deletion cancelled.");
						}
					}
					else
					{
						Console.WriteLine("Invalid Assignment Group ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Assignment Group ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No assignment groups to delete.");
			}
		}

		private static void AddAssignmentToGroup(Course selectedCourse)
		{
			if (!selectedCourse.AGroups.Any())
			{
				Console.WriteLine("No assignment groups available. Create an assignment group first.");
				return;
			}

			Console.WriteLine("\nAssignment Groups:");
			selectedCourse.AGroups.ForEach(Console.WriteLine);
			Console.WriteLine("Enter Assignment Group ID (or P to go back):");
			var agroupChoice = Console.ReadLine();

			if (agroupChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			else if (int.TryParse(agroupChoice, out int agroupId))
			{
				var selectedAGroup = selectedCourse.AGroups.FirstOrDefault(g => g.Id == agroupId);

				if (selectedAGroup != null)
				{
					if (selectedCourse.Assignments.Any())
					{
						Console.WriteLine("\nAvailable Assignments:");
						foreach (var assignment in selectedCourse.Assignments)
						{
							var inGroup = selectedAGroup.Assignments.Any(a => a.Id == assignment.Id) ? " (already in group)" : "";
							Console.WriteLine($"  [{assignment.Id}] {assignment.Name}{inGroup}");
						}
						Console.WriteLine("Enter Assignment ID to add to group (or P to cancel):");

						var assignmentChoice = Console.ReadLine();

						if (!assignmentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase)
							&& int.TryParse(assignmentChoice, out int assignmentId))
						{
							var assignmentToAdd = selectedCourse.Assignments.FirstOrDefault(a => a.Id == assignmentId);

							if (assignmentToAdd != null)
							{
								if (!selectedAGroup.Assignments.Any(a => a.Id == assignmentToAdd.Id))
								{
									selectedAGroup.Assignments.Add(assignmentToAdd);
									Console.WriteLine($"Successfully added '{assignmentToAdd.Name}' to group '{selectedAGroup.Name}'.");
								}
								else
								{
									Console.WriteLine("Assignment is already in this group.");
								}
							}
							else
							{
								Console.WriteLine("Invalid Assignment ID.");
							}
						}
					}
					else
					{
						Console.WriteLine("No assignments available. Create an assignment first.");
					}
				}
				else
				{
					Console.WriteLine("Invalid Assignment Group ID. Please try again.");
				}
			}
			else
			{
				Console.WriteLine("Invalid input. Please enter a valid Assignment Group ID or P to go back.");
			}
		}
		private static void ViewStudentFinalGrades(Course selectedCourse)
		{
			if (!selectedCourse.Students.Any())
			{
				Console.WriteLine("No students enrolled in this course.");
				return;
			}

			Console.WriteLine("\nEnrolled Students:");
			selectedCourse.Students.ForEach(Console.WriteLine);
			Console.WriteLine("Enter Student ID to view final grade (or P to go back):");

			var studentChoice = Console.ReadLine();

			if (studentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			else if (int.TryParse(studentChoice, out int studentId))
			{
				var selectedStudent = selectedCourse.Students.FirstOrDefault(s => s.Id == studentId);

				if (selectedStudent != null)
				{
					CalculateFinalGrade(selectedCourse, selectedStudent);
				}
				else
				{
					Console.WriteLine("Invalid Student ID. Please try again.");
				}
			}
			else
			{
				Console.WriteLine("Invalid input.");
			}
		}
		private static void CalculateFinalGrade(Course selectedCourse, Student student)
		{
			if (!selectedCourse.AGroups.Any())
			{
				Console.WriteLine("No assignment groups to calculate grades from.");
				return;
			}

			double totalWeightedGrade = 0;
			double totalWeight = 0;
			bool hasGrades = false;

			Console.WriteLine($"\nGrade Breakdown for {student.Name} in {selectedCourse.Name}:");
			Console.WriteLine("=".PadRight(50, '='));

			foreach (var agroup in selectedCourse.AGroups)
			{
				// Get all assignments in this group that belong to this course
				var groupAssignments = agroup.Assignments.Where(a => selectedCourse.Assignments.Contains(a)).ToList();

				if (!groupAssignments.Any())
				{
					Console.WriteLine($"{agroup.Name} (Weight: {agroup.Weight * 100}%): No assignments");
					continue;
				}

				// Calculate average grade for this group
				double totalPoints = 0;
				double earnedPoints = 0;
				int gradedCount = 0;

				foreach (var assignment in groupAssignments)
				{
					var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
					if (submission != null && submission.Grade.HasValue)
					{
						totalPoints += assignment.AvailablePoints;
						earnedPoints += submission.Grade.Value;
						gradedCount++;
						hasGrades = true;
					}
				}

				if (gradedCount > 0)
				{
					double groupPercentage = (earnedPoints / totalPoints) * 100;
					double weightedContribution = (earnedPoints / totalPoints) * agroup.Weight;

					Console.WriteLine($"{agroup.Name} (Weight: {agroup.Weight * 100}%): {groupPercentage:F2}% ({earnedPoints}/{totalPoints} points)");

					totalWeightedGrade += weightedContribution;
					totalWeight += agroup.Weight;
				}
				else
				{
					Console.WriteLine($"{agroup.Name} (Weight: {agroup.Weight * 100}%): No graded assignments yet");
				}
			}

			Console.WriteLine("=".PadRight(50, '='));

			if (hasGrades)
			{
				double finalGrade = (totalWeightedGrade / totalWeight) * 100;
				Console.WriteLine($"Final Grade: {finalGrade:F2}%");
				Console.WriteLine($"Letter Grade: {GetLetterGrade(finalGrade)}");
			}
			else
			{
				Console.WriteLine("No graded assignments yet. Cannot calculate final grade.");
			}
		}
		private static string GetLetterGrade(double percentage)
		{
			if (percentage >= 90) return "A";
			if (percentage >= 80) return "B";
			if (percentage >= 70) return "C";
			if (percentage >= 60) return "D";
			return "F";
		}

	}
}
using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;

namespace Library.LMS.Services
{
	public class AssignmentServiceProxy
	{
		private List<Assignment> assignments;
		public List<Assignment> Assignments
		{
			get
			{
				return assignments;
			}
			set
			{
				if (assignments != value)
				{
					assignments = value;
				}
			}
		}

		private static AssignmentServiceProxy? instance;
		private static object instanceLock = new object();

		public static AssignmentServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new AssignmentServiceProxy();
					}
				}
				return instance;
			}
		}

		private AssignmentServiceProxy()
		{
			assignments = new List<Assignment>
			{
			// Assignments
			 new Assignment { Id = 1, Name = "Syntax Mastery", Description = "The Grade Calculator", AvailablePoints = 100, DueDate = "2026-02-15" },
				new Assignment { Id = 2, Name = "Periodic Table Lab", Description = "Identify elements and their properties", AvailablePoints = 50, DueDate = "2026-02-22" },
				new Assignment { Id = 3, Name = "GE L1-1 (XはYです)", Description = "I am.../It is...", AvailablePoints = 25, DueDate = "2025-08-18" },
				new Assignment { Id = 4, Name = "Hello World Program", Description = "Write your first program", AvailablePoints = 50, DueDate = "2026-03-15" },
				new Assignment { Id = 5, Name = "Periodic Trends Lab Report", Description = "Test and analyze elemental reactivity patterns", AvailablePoints = 75, DueDate = "2026-03-15" },
				new Assignment { Id = 6, Name = "LC-L1 (New Friends)", Description = "Listen as Mary introduces herself in Japanese", AvailablePoints = 50, DueDate = "2025-10-15" },

				// Quizzes
				new Quiz { Id = 7, Name = "C# Basics Quiz", Question = "What is the difference between a class and a struct in C#?", AvailablePoints = 100, DueDate = "2026-02-20" },
				new Quiz { Id = 8, Name = "Elements Quiz", Question = "What is the atomic number of Carbon?", AvailablePoints = 100, DueDate = "2026-02-28" },
				new Quiz { Id = 9, Name = "Writing Quiz L1", Question = "In Japanese, write an introduction about yourself.", AvailablePoints = 100, DueDate = "2025-09-01" }
			};
		}

		public void Add(Assignment assignment)
		{
			assignment.Id = NextKey;
			Assignments.Add(assignment);
		}

		public int NextKey
		{
			get
			{
				if (Assignments.Any())
				{
					return Assignments.Select(a => a.Id).Max() + 1;
				}
				return 1;
			}
		}
	}
}
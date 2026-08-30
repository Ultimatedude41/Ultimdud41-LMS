using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CLI.LMS.Model
{
	public abstract class ContentPlus
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public abstract string Display();

		public override string ToString()
		{
			return $"{Id}. {Name}";
		}
	}

	public class AssignPlus : ContentPlus
	{
		public Assignment Assignment { get; set; }

		public override string Display()
		{
			if (Assignment != null)
			{
				return $"Assignment: {Assignment.Name}\n" +
					   $"Description: {Assignment.Description}\n" +
					   $"Points: {Assignment.AvailablePoints}\n" +
					   $"Due: {Assignment.DueDate}";
			}
			return "No assignment linked.";
		}

		public override string ToString()
		{
			return $"[Assignment] {Name}";
		}
	}

	public class FilePlus : ContentPlus
	{
		public string FilePath { get; set; }

		public override string Display()
		{
			return $"File: {Name}\nPath: {FilePath}";
		}

		public void OpenFile()
		{
			if (string.IsNullOrWhiteSpace(FilePath))
			{
				Console.WriteLine("Error: File path is empty.");
				return;
			}

			if (!File.Exists(FilePath))
			{
				Console.WriteLine($"Error: File not found at path: {FilePath}");
				return;
			}

			try
			{
				var psi = new ProcessStartInfo
				{
					FileName = FilePath,
					UseShellExecute = true
				};
				Process.Start(psi);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Could not open file: {ex.Message}");
			}
		}

		public override string ToString()
		{
			return $"[File] {Name}";
		}
	}

	public class Page : ContentPlus
	{
		public string Content { get; set; }

		public override string Display()
		{
			return Content;
		}

		public override string ToString()
		{
			return $"[Page] {Name}";
		}
	}
}
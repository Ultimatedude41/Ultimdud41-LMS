using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;

namespace Library.LMS.Services
{
	public class SubmitServiceProxy
	{
		private List<Submission> submissions;
		public List<Submission> Submissions
		{
			get
			{
				return submissions;
			}
			set
			{
				if (submissions != value)
				{
					submissions = value;
				}
			}
		}

		private static SubmitServiceProxy? instance;
		private static object instanceLock = new object();

		public static SubmitServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new SubmitServiceProxy();
					}
				}
				return instance;
			}
		}

		private SubmitServiceProxy()
		{
			submissions = new List<Submission>();
			// No preloaded submissions - students will create them
		}

		public void Add(Submission submission)
		{
			submission.Id = NextKey;
			Submissions.Add(submission);
		}

		public int NextKey
		{
			get
			{
				if (Submissions.Any())
				{
					return Submissions.Select(s => s.Id).Max() + 1;
				}
				return 1;
			}
		}
	}
}
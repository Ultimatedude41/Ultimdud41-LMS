using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CLI.LMS.Model;

namespace Library.LMS.Services
{
	public class AGroupServiceProxy
	{
		private List<AGroup> agroups;
		public List<AGroup> AGroups
		{
			get
			{
				return agroups;
			}
			set
			{
				if (agroups != value)
				{
					agroups = value;
				}
			}
		}

		private static AGroupServiceProxy? instance;
		private static object instanceLock = new object();

		public static AGroupServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new AGroupServiceProxy();
					}
				}
				return instance;
			}
		}

				private AGroupServiceProxy()
				{
					// Get assignments from AssignmentServiceProxy
					var assignments = AssignmentServiceProxy.Current.Assignments;

					agroups = new List<AGroup>
			{
				new AGroup
				{
					Id = 1,
					Name = "Core Projects",
					Weight = 0.30,  // 30%
					Assignments = new List<Assignment> { assignments[0] }
				},
				new AGroup
				{
					Id = 2,
					Name = "Labs",
					Weight = 0.20,  // 20%
					Assignments = new List<Assignment> { assignments[1] }
				},
				new AGroup
				{
					Id = 3,
					Name = "Grammar Exercises",
					Weight = 0.20,  // 20%
					Assignments = new List<Assignment> { assignments[2] }
				},
				new AGroup
				{
					Id = 4,
					Name = "Mini Codes",
					Weight = 0.20,  // 20%
                    Assignments = new List<Assignment> { assignments[3] }
                },
								new AGroup
				{
					Id = 5,
					Name = "Reports",
					Weight = 0.40,  // 40%
                    Assignments = new List<Assignment> { assignments[4] }
                },
								new AGroup
				{
					Id = 6,
					Name = "Listening Comprehension",
					Weight = 0.30,  // 30%
                    Assignments = new List<Assignment> { assignments[5] }
                },
								new AGroup
				{
					Id = 7,
					Name = "C# Quizzes",
					Weight = 0.50,  // 50%
                    Assignments = new List<Assignment> { assignments[6] }
				},
								new AGroup
				{
					Id = 8,
					Name = "Chem. Quizzes",
					Weight = 0.40,  // 40%
                    Assignments = new List<Assignment> { assignments[7] }
				},
								new AGroup
				{
					Id = 9,
					Name = "Japanese Quizzes",
					Weight = 0.50,  // 50%
                    Assignments = new List<Assignment> { assignments[8] }
				}
			};
		}

		public void Add(AGroup agroup)
		{
			agroup.Id = NextKey;
			AGroups.Add(agroup);
		}

		public int NextKey
		{
			get
			{
				if (AGroups.Any())
				{
					return AGroups.Select(g => g.Id).Max() + 1;
				}
				return 1;
			}
		}
	}
}
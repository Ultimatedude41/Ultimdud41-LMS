using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLI.LMS.Model;

namespace Library.LMS.Services
{
	public class ModuleServiceProxy
	{
		private List<Module> modules;
		public List<Module> Modules
		{
			get
			{
				return modules;
			}
			set
			{
				if (modules != value)
				{
					modules = value;
				}
			}
		}

		private static ModuleServiceProxy? instance;
		private static object instanceLock = new object();

		public static ModuleServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new ModuleServiceProxy();
					}
				}
				return instance;
			}
		}

		private ModuleServiceProxy()
		{
			modules = new List<Module>
			{
				new Module
				{
					Id = 1,
					Content = new List<ContentPlus>
					{
						new Page { Id = 1, Name = "Introduction", Content = "What is programming?" },
						new Page { Id = 2, Name = "Setup Guide", Content = "Setting up your IDE" }
					}
				},
				new Module
				{
					Id = 2,
					Content = new List<ContentPlus>
					{
						new Page { Id = 3, Name = "Chemistry Basics", Content = "What is chemistry?" },
						new Page { Id = 4, Name = "Periodic Table", Content = "Understanding the periodic table" }
					}
				},
				new Module
				{
					Id = 3,
					Content = new List<ContentPlus>
					{
						new Page { Id = 5, Name = "Writing Systems", Content = "Hiragana, Katakana, and basic stroke order" },
						new Page { Id = 6, Name = "Daily Essentials", Content = "Self-introductions, greetings, and numbers 1-100" }
					}
				}
			};
		}

		public void Add(Module module)
		{
			module.Id = NextKey;
			Modules.Add(module);
		}

		public int NextKey
		{
			get
			{
				if (Modules.Any())
				{
					return Modules.Select(m => m.Id).Max() + 1;
				}
				return 1;
			}
		}

		public int NextContentKey
		{
			get
			{
				int maxId = 0;
				foreach (var module in Modules)
				{
					foreach (var content in module.Content)
					{
						if (content.Id > maxId)
						{
							maxId = content.Id;
						}
					}
				}
				return maxId + 1;
			}
		}
	}
}
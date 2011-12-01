// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Text;
using System.Web;

namespace Tp.Integration.Plugin.TaskCreator
{
	public class TaskCreatorProfileWrapper
	{
		private readonly TaskCreatorProfile _profile;

		public TaskCreatorProfileWrapper(TaskCreatorProfile profile)
		{
			_profile = profile;
		}

		public string CommandName
		{
			get { return HttpUtility.UrlDecode(_profile.CommandName, Encoding.UTF7); }
		}

		public string TasksList
		{
			get { return HttpUtility.UrlDecode(_profile.TasksList, Encoding.UTF7); }
		}

		public int Project
		{
			get { return _profile.Project; }
		}
	}
}
//------------------------------------------------------------------------------
// <copyright company="LeanKit Inc.">
//     Copyright (c) LeanKit Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Linq;
using LeanKit.API.Client.Library.TransferObjects;
using net.sf.mpxj;
using net.sf.mpxj.ExtensionMethods;

namespace IntegrationService.Targets.MicrosoftProject
{
    public static class MicrosoftProjectConversionExtensions
    {

        public static int LeanKitPriority(this Task task)
        {
           return CalculateLeanKitPriority(task);
        }

		public static int CalculateLeanKitPriority(Task task) 
		{
			//LK Priority: 0 = Low, 1 = Normal, 2 = High, 3 = Critical
			//Unfuddle Priority: 100 = LOWEST, 200 = VERY_LOW, 300 = LOWER, 400 = LOW, 
			//					 500 = MEDIUM, 600 = HIGH, 700 = HIGHER, 800 = VERY_HIGH,
			//					 900 = HIGHEST, 1000 = DO_NOT_LEVEL

			int lkPriority = 1; // default to 1 - Normal
			if (task == null)
				return lkPriority;

			switch (task.Priority.Value) 
			{
				case 900:
				case 800:
					return 3;
					break;
				case 700:
				case 600:
					return 2;
					break;
				case 400:
				case 300:
				case 200:
				case 100:
					return 0;
					break;
				case 500:
				default:
					return 1;
					break;
			}
		}

        public static CardType LeanKitCardType(this Task task, BoardMapping project)
        {
            return CalculateLeanKitCardType(project, "");
        }

		public static CardType CalculateLeanKitCardType(BoardMapping project, string issueTypeName) 
		{
			var boardId = project.Identity.LeanKit;

			if (!string.IsNullOrEmpty(issueTypeName)) 
			{
				var mappedWorkType = project.Types.FirstOrDefault(x => x.Target.ToLowerInvariant() == issueTypeName.ToLowerInvariant());
				if (mappedWorkType != null) {
					var definedVal = project.ValidCardTypes.FirstOrDefault(x => x.Name.ToLowerInvariant() == mappedWorkType.LeanKit.ToLowerInvariant());
					if (definedVal != null) {
						return definedVal;
					}
				}
				var implicitVal = project.ValidCardTypes.FirstOrDefault(x => x.Name.ToLowerInvariant() == issueTypeName.ToLowerInvariant());
				if (implicitVal != null) {
					return implicitVal;
				}
			}

			return project.ValidCardTypes.FirstOrDefault(x => x.IsDefault);
		}

		public static DateTime? GetDueDate(this Task task)
		{
			if (task.BaselineFinish != null) 
			{
				return task.BaselineFinish.ToDateTime();
			} 
			if (task.Finish != null) 
			{
				return task.Finish.ToDateTime();
			} 
			if (task.EarlyFinish != null) 
			{
				return task.EarlyFinish.ToDateTime();
			}
			return null;
		}

		public static DateTime? GetStartDate(this Task task)
		{
			if (task.BaselineStart != null) 
			{
				return task.BaselineStart.ToDateTime();
			} 
			if (task.Start != null) 
			{
				return task.Start.ToDateTime();
			} 
			if (task.EarlyStart != null) 
			{
				return task.EarlyStart.ToDateTime();
			}
			return null;
		}

		public static int GetSize(this Task task)
		{
			if (task.BaselineWork != null && task.BaselineWork.Duration >= 1) 
			{
				return (int) task.BaselineWork.Duration;
			} 
			if (task.Work != null && task.Work.Duration >= 1) 
			{
				return (int)task.Work.Duration;
			} 
			if (task.BaselineCost.intValue() >= 1) 
			{
				return task.BaselineCost.intValue();
			} 
			if (task.Cost.intValue() >= 1) 
			{
				return task.BaselineCost.intValue();
			}
			return 0;
		}
    }
}
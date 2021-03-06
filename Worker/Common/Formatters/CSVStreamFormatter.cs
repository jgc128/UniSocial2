﻿using Collector.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collector.Common;


namespace Worker.Common.Formatters
{
	public class CSVStreamFormatter : ObjectFormatter
	{
		static Dictionary<Type, Action<object, StreamWriter>> formatters;
		Action<object, StreamWriter> currentFormatter;

		MemoryStream resultStream;
		StreamWriter writer;

		static CSVStreamFormatter()
		{
			formatters = new Dictionary<Type, Action<object, StreamWriter>>();

			formatters.Add(typeof(VkUser), formatVkUser);
			formatters.Add(typeof(VkGroup), formatVkGroup);
			formatters.Add(typeof(VkPost), formatVkPostWithCounts);
			formatters.Add(typeof(VkUserSubscriptions), formatVkUserSubscriptions);
			formatters.Add(typeof(VkGroupMembers), formatVkGroupMembers);
			formatters.Add(typeof(VkFriends), formatVkFriends);
			formatters.Add(typeof(VkUserGroups), formatVkUserGroups);
			formatters.Add(typeof(VkWallComments), formatVkComment);
		}

		#region Fromatters

		private static void formatVkPost(object o, StreamWriter s)
		{
			var obj = o as VkPost;

			s.Write("\""); s.Write(obj.Id); s.Write("\",");
			s.Write("\""); s.Write(obj.ToId); s.Write("\",");
			s.Write("\""); s.Write(obj.FromId); s.Write("\",");
			s.Write("\""); s.Write(obj.Date); s.Write("\",");

			if (obj.CopyHistory != null && obj.CopyHistory.Count > 0)
			{
				var copyPost = obj.CopyHistory.First();

				s.Write("\""); s.Write(copyPost.Text.Replace("\"", "\"\"")); s.Write("\","); // Text - from copy post
				s.Write("\""); s.Write(obj.FromId); s.Write("\",");

				s.Write("\""); s.Write(copyPost.Date.ToUnixTimestamp()); s.Write("\",");
				s.Write("\""); s.Write(copyPost.FromId); s.Write("\",");
				s.Write("\""); s.Write(copyPost.Id); s.Write("\",");
				s.Write("\""); s.Write(obj.Text.Replace("\"", "\"\"")); s.Write("\"");
			}
			else
			{
				s.Write("\""); s.Write(obj.Text.Replace("\"", "\"\"")); s.Write("\",");
				s.Write("\""); s.Write(obj.FromId); s.Write("\",");

				s.Write("\""); s.Write(0); s.Write("\","); // copy_post_dae
				s.Write("\""); s.Write(0); s.Write("\","); // copy_owner_id
				s.Write("\""); s.Write(0); s.Write("\","); // copy_post_id
				s.Write("\""); s.Write(""); s.Write("\""); // copy_text
			}

			s.Write("\n");
		}
		private static void formatVkPostWithCounts(object o, StreamWriter s)
		{
			var obj = o as VkPost;

			s.Write("\""); s.Write(obj.Id); s.Write("\",");
			s.Write("\""); s.Write(obj.FromId); s.Write("\",");
			s.Write("\""); s.Write(obj.ToId); s.Write("\",");
			s.Write("\""); s.Write(obj.Date.ToString("o")); s.Write("\",");
			s.Write("\""); s.Write(obj.PostType.ToString()); s.Write("\",");
			s.Write("\""); s.Write(obj.Text.Replace("\"", "\"\"")); s.Write("\",");
			s.Write("\""); s.Write(obj.Comments != null ? obj.Comments.Count : 0); s.Write("\",");
			s.Write("\""); s.Write(obj.Likes != null ? obj.Likes.Count : 0); s.Write("\",");
			s.Write("\""); s.Write(obj.Reposts != null ? obj.Reposts.Count : 0); s.Write("\",");


			if (obj.CopyHistory != null && obj.CopyHistory.Count > 0)
			{
				var copyPost = obj.CopyHistory.First();

				s.Write("\""); s.Write(copyPost.Id); s.Write("\",");
				s.Write("\""); s.Write(copyPost.FromId); s.Write("\",");
				s.Write("\""); s.Write(copyPost.ToId); s.Write("\",");
				s.Write("\""); s.Write(copyPost.Text.Replace("\"", "\"\"")); s.Write("\"");
			}
			else
			{
				s.Write("\""); s.Write(0); s.Write("\",");
				s.Write("\""); s.Write(0); s.Write("\",");
				s.Write("\""); s.Write(0); s.Write("\",");
				s.Write("\""); s.Write(""); s.Write("\"");
			}

			s.Write("\n");
		}
		private static void formatVkGroup(object o, StreamWriter s)
		{
			var obj = o as VkGroup;

			s.Write("\""); s.Write(obj.Id); s.Write("\",");
			s.Write("\""); s.Write(obj.Name.Replace("\"", "\"\"")); s.Write("\",");
			s.Write("\""); s.Write(obj.ScreenName != null ? obj.ScreenName.Replace("\"", "\"\"") : ""); s.Write("\",");
			s.Write("\""); s.Write(obj.IsClosed); s.Write("\",");
			s.Write("\""); s.Write((int)obj.Type); s.Write("\",");
			s.Write("\""); s.Write(obj.MembersCount); s.Write("\"\n");
		}
		private static void formatVkUser(object o, StreamWriter s)
		{
			var obj = o as VkUser;

			s.Write("\""); s.Write(obj.Id); s.Write("\",");
			s.Write("\""); s.Write(obj.FirstName.Replace("\"", "\"\"")); s.Write("\",");
			s.Write("\""); s.Write(obj.LastName.Replace("\"", "\"\"")); s.Write("\",");
			s.Write("\""); s.Write(obj.ScreenName != null ? obj.ScreenName.Replace("\"", "\"\"") : ""); s.Write("\",");
			s.Write("\""); s.Write(obj.Nickname != null ? obj.ScreenName.Replace("\"", "\"\"") : ""); s.Write("\",");

			s.Write("\""); s.Write((int)obj.Sex); s.Write("\",");
			s.Write("\""); s.Write(obj.BDate); s.Write("\",");
			s.Write("\""); s.Write(obj.City); s.Write("\",");
			s.Write("\""); s.Write(obj.Country); s.Write("\",");
			s.Write("\""); s.Write(obj.Timezone); s.Write("\",");

			s.Write("\""); s.Write(obj.Photo50); s.Write("\",");
			s.Write("\""); s.Write(obj.Photo100); s.Write("\",");
			s.Write("\""); s.Write(obj.PhotoMaxOrig); s.Write("\",");

			s.Write("\""); s.Write(obj.HasMobile); s.Write("\",");
			s.Write("\""); s.Write(obj.HomePhone != null ? obj.HomePhone.Replace("\"", "\"\"") : ""); s.Write("\",");
			s.Write("\""); s.Write(obj.MobilePhone != null ? obj.MobilePhone.Replace("\"", "\"\"") : ""); s.Write("\",");

			s.Write("\""); s.Write(obj.University); s.Write("\",");
			s.Write("\""); s.Write(obj.University == 0 ? "" : obj.UniversityName.Replace("\"", "\"\"")); s.Write("\",");
			s.Write("\""); s.Write(obj.Faculty); s.Write("\",");
			s.Write("\""); s.Write(obj.Faculty == 0 ? "" : obj.FacultyName.Replace("\"", "\"\"")); s.Write("\",");
			s.Write("\""); s.Write(obj.Graduation); s.Write("\"\n");
		}
		private static void formatVkUserSubscriptions(object o, StreamWriter s)
		{
			var obj = o as VkUserSubscriptions;

			foreach (var grp in obj.Groups.Items)
			{
				s.Write(grp); s.Write(","); s.Write(obj.Id); s.Write("\n");
			}
		}
		private static void formatVkGroupMembers(object o, StreamWriter s)
		{
			var obj = o as VkGroupMembers;

			foreach (var memberId in obj.GroupMembers)
			{
				s.Write(obj.GroupId); s.Write(","); s.Write(memberId); s.Write("\n");
			}
		}
		private static void formatVkFriends(object o, StreamWriter s)
		{
			var obj = o as VkFriends;

			foreach (var friendId in obj.Friends)
			{
				s.Write(obj.UserId); s.Write(","); s.Write(friendId); s.Write("\n");
			}
		}
		private static void formatVkUserGroups(object o, StreamWriter s)
		{
			var obj = o as VkUserGroups;

			foreach (var groupId in obj.Groups)
			{
				s.Write(obj.UserId); s.Write(","); s.Write(groupId); s.Write("\n");
			}
		}

		private static void formatVkComment(object o, StreamWriter s)
		{
			var obj = o as VkWallComments;

			foreach (var comment in obj.Comments)
			{
				s.Write("\""); s.Write(obj.OwnerId); s.Write("\",");
				s.Write("\""); s.Write(obj.PostId); s.Write("\",");
				s.Write("\""); s.Write(comment.Id); s.Write("\",");
				s.Write("\""); s.Write(comment.FromId); s.Write("\",");
				s.Write("\""); s.Write(comment.Date.ToString("o")); s.Write("\",");
				s.Write("\""); s.Write(comment.Text.Replace("\"", "\"\"")); s.Write("\",");
				s.Write("\""); s.Write(comment.Likes != null ? comment.Likes.Count : 0); s.Write("\"\n");
			}
		}

		#endregion

		public CSVStreamFormatter()
		{
		}

		protected override void SetObjectType(Type t)
		{
			currentFormatter = formatters[t];
			resultStream = new MemoryStream();
			writer = new StreamWriter(resultStream);
		}

		protected override void HandleObject(object Obj)
		{
			currentFormatter(Obj, writer);
		}

		protected override object GetResult()
		{
			writer.Flush();
			return resultStream;
		}

		public override void Dispose()
		{
			writer.Close();
			resultStream.Close();
		}
	}
}

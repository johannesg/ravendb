//-----------------------------------------------------------------------
// <copyright file="WillNotFailSystemIfServerIsNotAvailableOnStartup.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Management;
using Raven.Client.Document;
using Raven.Database.Config;
using Raven.Database.Extensions;
using Raven.Database.Server;
using Raven.Tests.Document;
using Xunit;

namespace Raven.Tests.Bugs
{
	public class WillNotFailSystemIfServerIsNotAvailableOnStartup : RemoteClientTest, IDisposable
	{
		private string path;

		#region IDisposable Members

		public void Dispose()
		{
			IOExtensions.DeleteDirectory(path);
		}

		#endregion


		public WillNotFailSystemIfServerIsNotAvailableOnStartup()
		{
			path = GetPath("TestDb");
			NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8079);
		}

		[Fact]
		public void CanStartWithoutServer()
		{
			using (var store = new DocumentStore
			{
				Url = "http://localhost:8079"
			}.Initialize())
			{
				Assert.Throws<WebException>(() => store.OpenSession().Load<User>("user/1"));
				using (GetNewServer(8079,path))
				{
					using (var session = store.OpenSession())
					{
						var person = session.Load<HierarchyFromClient.Person>("people/1");
						Assert.Null(person);
					}
				}
			}
		}
	}
}
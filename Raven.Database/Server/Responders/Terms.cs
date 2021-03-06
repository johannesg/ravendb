//-----------------------------------------------------------------------
// <copyright file="Terms.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven.Database.Extensions;
using Raven.Database.Queries;
using Raven.Database.Server.Abstractions;

namespace Raven.Database.Server.Responders
{
	public class Terms : RequestResponder
	{
		public override string UrlPattern
		{
			get { return "^/terms/(.+)"; }
		}

		public override string[] SupportedVerbs
		{
			get { return new []{"GET"}; }
		}

		public override void Respond(IHttpContext context)
		{
			var match = urlMatcher.Match(context.GetRequestUrl());
			var index = match.Groups[1].Value;

			var indexEtag = Database.GetIndexEtag(index);
			if (context.MatchEtag(indexEtag))
			{
				context.SetStatusToNotModified();
				return;
			}
			
			var executeGetTermsQuery = Database.ExecuteGetTermsQuery(index, 
			                                                         context.Request.QueryString["field"],
			                                                         context.Request.QueryString["fromValue"],
			                                                         context.GetPageSize(Database.Configuration.MaxPageSize)
				);
			context.Response.AddHeader("ETag", Database.GetIndexEtag(index).ToString());
			context.WriteJson(executeGetTermsQuery);
		}
	}
}
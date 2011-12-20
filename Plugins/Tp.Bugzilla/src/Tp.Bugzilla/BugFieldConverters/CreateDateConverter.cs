// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Tp.Core;

namespace Tp.Bugzilla.BugFieldConverters
{
	public class CreateDateConverter : IBugConverter
	{
		private static readonly Regex DateRegex = new Regex(@"(?<dateTime>.*?)(?<offset>([+-]\d\d\d\d)|[a-z]+)\s*$",
		                                                    RegexOptions.IgnoreCase);

		public void Apply(BugzillaBug bugzillaBug, ConvertedBug convertedBug)
		{
			convertedBug.BugDto.CreateDate = ParseFromBugzillaLocalTime(bugzillaBug.creation_ts);
		}

		public static DateTime Parse(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			var m = DateRegex.Match(input);
			if (m.Success)
			{
				input = m.Groups["dateTime"].Value.Trim();
			}

			return DateTime.ParseExact(input, _formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite);
		}

		private static TimeSpan GetOffset(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			int zzz = 0;

			var m = DateRegex.Match(input);
			if (m.Success)
			{
				var zone = m.Groups["offset"].Value;
				switch (zone.ToLower())
				{
					case "brst":
						zzz = -0200;
						break; // Brazil Summer Time (East Daylight)
					case "adt":
						zzz = -0300;
						break; // Atlantic Daylight
					case "edt":
						zzz = -0400;
						break; // Eastern Daylight
					case "cdt":
						zzz = -0500;
						break; // Central Daylight
					case "mdt":
						zzz = -0600;
						break; // Mountain Daylight
					case "pdt":
						zzz = -0700;
						break; // Pacific Daylight
					case "ydt":
						zzz = -0800;
						break; // Yukon Daylight
					case "hdt":
						zzz = -0900;
						break; // Hawaii Daylight
					case "bst":
						zzz = +0100;
						break; // British Summer   
					case "mest":
						zzz = +0200;
						break; // Middle European Summer   
					case "sst":
						zzz = +0200;
						break; // Swedish Summer
					case "fst":
						zzz = +0200;
						break; // French Summer
					case "cest":
						zzz = +0200;
						break; // Central European Daylight
					case "eest":
						zzz = +0300;
						break; // Eastern European Summer
					case "wadt":
						zzz = +0800;
						break; // West Australian Daylight
					case "kdt":
						zzz = +1000;
						break; // Korean Daylight
					case "eadt":
						zzz = +1100;
						break; // Eastern Australian Daylight
					case "nzd":
						zzz = +1300;
						break; // New Zealand Daylight   
					case "nzdt":
						zzz = +1300;
						break; // New Zealand Daylight   
					case "gmt":
						zzz = 000;
						break; // Greenwich Mean
					case "ut":
						zzz = 000;
						break; // Universal (Coordinated)
					case "utc":
						zzz = 000;
						break;
					case "wet":
						zzz = 000;
						break; // Western European
					case "wat":
						zzz = -0100;
						break; // West Africa
					case "at":
						zzz = -0200;
						break; // Azores
					case "fnt":
						zzz = -0200;
						break; // Brazil Time (Extreme East - Fernando Noronha)
					case "brt":
						zzz = -0300;
						break; // Brazil Time (East Standard - Brasilia)
					case "mnt":
						zzz = -0400;
						break; // Brazil Time (West Standard - Manaus)
					case "ewt":
						zzz = -0400;
						break; // U.S. Eastern War Time
					case "ast":
						zzz = -0400;
						break; // Atlantic Standard
					case "est":
						zzz = -0500;
						break; // Eastern Standard
					case "act":
						zzz = -0500;
						break; // Brazil Time (Extreme West - Acre)
					case "cst":
						zzz = -0600;
						break; // Central Standard
					case "mst":
						zzz = -0700;
						break; // Mountain Standard
					case "pst":
						zzz = -0800;
						break; // Pacific Standard
					case "yst":
						zzz = -0900;
						break; // Yukon Standard
					case "hst":
						zzz = -1000;
						break; // Hawaii Standard
					case "cat":
						zzz = -1000;
						break; // Central Alaska
					case "ahst":
						zzz = -1000;
						break; // Alaska-Hawaii Standard
					case "nt":
						zzz = -1100;
						break; // Nome
					case "idlw":
						zzz = -1200;
						break; // International Date Line West
					case "cet":
						zzz = +0100;
						break; // Central European
					case "mez":
						zzz = +0100;
						break; // Central European (German)
					case "ect":
						zzz = +0100;
						break; // Central European (French)
					case "met":
						zzz = +0100;
						break; // Middle European
					case "mewt":
						zzz = +0100;
						break; // Middle European Winter
					case "swt":
						zzz = +0100;
						break; // Swedish Winter
					case "set":
						zzz = +0100;
						break; // Seychelles
					case "fwt":
						zzz = +0100;
						break; // French Winter
					case "eet":
						zzz = +0200;
						break; // Eastern Europe, USSR Zone 1
					case "ukr":
						zzz = +0200;
						break; // Ukraine
					case "bt":
					case "msk":
						zzz = +0300;
						break; // Baghdad, USSR Zone 2, Moscow
					case "wst":
						zzz = +0800;
						break; // West Australian Standard
					case "hkt":
						zzz = +0800;
						break; // Hong Kong
					case "cct":
						zzz = +0800;
						break; // China Coast, USSR Zone 7
					case "jst":
						zzz = +0900;
						break; // Japan Standard, USSR Zone 8
					case "kst":
						zzz = +0900;
						break; // Korean Standard
					case "east":
						zzz = +1000;
						break; // Eastern Australian Standard
					case "gst":
						zzz = +1000;
						break; // Guam Standard, USSR Zone 9
					case "nzt":
						zzz = +1200;
						break; // New Zealand
					case "nzst":
						zzz = +1200;
						break; // New Zealand Standard
					case "idle":
						zzz = +1200;
						break; // International Date Line East

					default:
						{
							if (!int.TryParse(zone, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out zzz))
							{
								zzz = 0;
							}
							break;
						}
				}
			}

			return new TimeSpan(zzz/100, zzz%100, 0);
		}

		public static DateTime ParseToUniversalTime(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			var time = Parse(input);
			var offset = GetOffset(input);
			var dateTime = time.Subtract(offset);

			return dateTime;
		}

		public static DateTime ParseFromBugzillaLocalTime(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			var time = Parse(input);
			var offset = GetOffset(input);
			var dateTime = time.Subtract(offset).ToLocalTime();

			return dateTime <= CurrentDate.Value ? dateTime : CurrentDate.Value;
		}

		private static readonly string[] _formats =
			{
				// these 2 time formats are used in Bugzilla
				"yyyy'-'MM'-'dd HH':'mm",
				"yyyy'-'MM'-'dd HH':'mm':'ss",
				"dd MMM yyyy HH':'mm",
				"dd MMM yyyy HH':'mm':'ss",
				"ddd, dd MMM yyyy HH':'mm",
				"ddd, dd MMM yyyy HH':'mm':'ss",
			};
	}
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="APIHelper.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   APIHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Reflection;
	using System.Text;

	using Newtonsoft.Json;

	using Sharlayan.Models;
	using Sharlayan.Models.Structures;
	using Sharlayan.Models.XIVDatabase;

	using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

	internal static class APIHelper {
		private static WebClient _webClient = new WebClient {
			Encoding = Encoding.UTF8
		};

		public static string LoadJsonResource(string resourceName) {
			try {
				return (string) Properties.Resources.ResourceManager.GetObject(Path.GetFileNameWithoutExtension(resourceName));
			} catch {
			}
			return string.Empty;
		}

		public static void GetActions(ConcurrentDictionary<uint, ActionItem> actions, string patchVersion = "latest") {
			var filename = "actions.json";
			var file = Path.Combine(Sharlayan.Reader.JsonPath, filename);
			if(File.Exists(file) && MemoryHandler.Instance.UseLocalCache) {
				EnsureDictionaryValues(actions, FileResponseToJSON(file));
			} else {
				EnsureDictionaryValues(actions, LoadJsonResource(filename));
			}
		}

		public static IEnumerable<Signature> GetSignatures() {
			var filename = "signatures.json";
			var file = Path.Combine(Sharlayan.Reader.JsonPath, filename);
			if(File.Exists(file) && MemoryHandler.Instance.UseLocalCache) {
				var json = FileResponseToJSON(file);
				return JsonConvert.DeserializeObject<IEnumerable<Signature>>(json, Constants.SerializerSettings);
			} else {
				var json = LoadJsonResource(filename);
				return JsonConvert.DeserializeObject<IEnumerable<Signature>>(json, Constants.SerializerSettings);
			}
		}

		public static void GetStatusEffects(ConcurrentDictionary<uint, StatusItem> statusEffects, string patchVersion = "latest") {
			var filename = "statuses.json";
			var file = Path.Combine(Sharlayan.Reader.JsonPath, filename);
			if(File.Exists(file) && MemoryHandler.Instance.UseLocalCache) {
				EnsureDictionaryValues(statusEffects, FileResponseToJSON(file));
			} else {
				EnsureDictionaryValues(statusEffects, LoadJsonResource(filename));
			}
		}

		public static StructuresContainer GetStructures(ProcessModel processModel, string patchVersion = "latest") {
			var filename = "structures.json";
			var file = Path.Combine(Sharlayan.Reader.JsonPath, filename);
			if(File.Exists(file) && MemoryHandler.Instance.UseLocalCache) {
				return EnsureClassValues<StructuresContainer>(file);
			}

			return JsonConvert.DeserializeObject<StructuresContainer>(LoadJsonResource(filename), Constants.SerializerSettings);
		}

		public static void GetZones(ConcurrentDictionary<uint, MapItem> mapInfos, string patchVersion = "latest") {
			// These ID's link to offset 7 in the old JSON values.
			// eg: "map id = 4" would be 148 in offset 7.
			// This is known as the TerritoryType value
			// - It maps directly to SaintCoins map.csv against TerritoryType ID
			var filename = "zones.json";
			var file = Path.Combine(Sharlayan.Reader.JsonPath, filename);
			if(File.Exists(file) && MemoryHandler.Instance.UseLocalCache) {
				EnsureDictionaryValues(mapInfos, FileResponseToJSON(file));
			} else {
				EnsureDictionaryValues(mapInfos, LoadJsonResource(filename));
			}
		}


		private static T EnsureClassValues<T>(string file) {
			var json = FileResponseToJSON(file);
			return JsonConvert.DeserializeObject<T>(json, Constants.SerializerSettings);
		}

		private static void EnsureDictionaryValues<T>(ConcurrentDictionary<uint, T> dictionary, string json) {
			ConcurrentDictionary<uint, T> resolved = JsonConvert.DeserializeObject<ConcurrentDictionary<uint, T>>(json, Constants.SerializerSettings);

			foreach(KeyValuePair<uint, T> kvp in resolved) {
				dictionary.AddOrUpdate(kvp.Key, kvp.Value, (k, v) => kvp.Value);
			}
		}

		private static string FileResponseToJSON(string file) {
			using(var streamReader = new StreamReader(file)) {
				return streamReader.ReadToEnd();
			}
		}
	}
}
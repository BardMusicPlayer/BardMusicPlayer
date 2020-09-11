using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public static class Extensions {

    public static string Repeat(this string s, int n)
        => new StringBuilder(s.Length * n).Insert(0, s, n).ToString();

	public static string ToQuery(this Dictionary<string, string> dict) {
		string query = string.Empty;
		var validParameters = dict.Where(p => !String.IsNullOrEmpty(p.Value));
		var formattedParameters = validParameters.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value));
		query += string.Join("&", formattedParameters.ToArray());
		return query;
	}
	public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
		if(val == null)
			throw new ArgumentNullException(nameof(val), "is null.");
		if(min == null)
			throw new ArgumentNullException(nameof(min), "is null.");
		if(max == null)
			throw new ArgumentNullException(nameof(max), "is null.");
		//If min <= max, clamp
		if(min.CompareTo(max) <= 0)
			return val.CompareTo(min) < 0 ? min : val.CompareTo(max) > 0 ? max : val;
		//If min > max, clamp on swapped min and max
		return val.CompareTo(max) < 0 ? max : val.CompareTo(min) > 0 ? min : val;
	}
	public static string FirstCharToUpper(this string input) {
		switch (input) {
			case null: throw new ArgumentNullException(nameof(input));
			case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
			default: return input.First().ToString().ToUpper() + input.Substring(1);
		}
	}
	public static byte[] ToByteArray(this Object obj) {
		BinaryFormatter bf = new BinaryFormatter();
		using (var ms = new MemoryStream()) {
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}
	}
	public static Object ToObject(this byte[] arrBytes) {
		using (var memStream = new MemoryStream()) {
			var binForm = new BinaryFormatter();
			memStream.Write(arrBytes, 0, arrBytes.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			var obj = binForm.Deserialize(memStream);
			return obj;
		}
	}

}
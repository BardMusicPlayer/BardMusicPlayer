// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
	using System;
	
	using Sharlayan.Core;
	using Sharlayan.Core.Enums;

	public static partial class Reader {
		public static bool CanGetChatInput() {
			var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.ChatInputKey);
			if(canRead) {
				// OTHER STUFF?
			}
			return canRead;
		}

		public static bool IsChatInputOpen() {
			if(!CanGetChatInput() || !MemoryHandler.Instance.IsAttached) {
				return false;
			}
			try {
				var ChatInputMap = (IntPtr) Scanner.Instance.Locations[Signatures.ChatInputKey];
				bool pointer = ((IntPtr) MemoryHandler.Instance.GetInt32(ChatInputMap)) != IntPtr.Zero;
				return pointer;
			} catch(Exception ex) {
				MemoryHandler.Instance.RaiseException(Logger, ex, true);
			}
			return false;
		}

		public static string GetChatInputString() {
			string chatString = string.Empty;
			if(!CanGetChatInput() || !MemoryHandler.Instance.IsAttached || !IsChatInputOpen()) {
				return chatString;
			}
			try {
				var ChatInputMap = (IntPtr) Scanner.Instance.Locations[Signatures.ChatInputKey];
				IntPtr chatPointer = ((IntPtr) MemoryHandler.Instance.GetInt64(ChatInputMap));

				int chatLen = MemoryHandler.Instance.GetInt16(chatPointer, -0xF0);
				if(chatLen <= 501) {
					chatPointer = new IntPtr(MemoryHandler.Instance.GetInt64(chatPointer, -0x100));
					if(chatPointer != IntPtr.Zero) {
						chatString = MemoryHandler.Instance.GetString(chatPointer, 0, chatLen);
					}
				}
				return chatString;
			} catch(Exception ex) {
				MemoryHandler.Instance.RaiseException(Logger, ex, true);
			}
			return chatString;
		}
	}
}
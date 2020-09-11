// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.CurrentPlayer.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.CurrentPlayer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using FFBardMusicCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FFMemoryParser {

    public class SignatureActors : Signature {
        private ActorDataList tempActors = new ActorDataList();
        private Dictionary<uint, DateTime> expiringActors = new Dictionary<uint, DateTime>();

        public SignatureActors(Signature sig) : base(sig) { }

        public override object GetData(HookProcess process) {

            if(baseAddress.ToInt64() <= 6496) {
                return null;
            }
            if(!Offsets.ContainsKey("SourceSize") || !Offsets.ContainsKey("EntityCount")) {
                Console.WriteLine("Couldn't find basic");
                return null;
            }

            if (!Offsets.ContainsKey("ID") || !Offsets.ContainsKey("Type")) {
                Console.WriteLine("Couldn't find player basic");
                return null;
            }

            var data = new SigActorsData();

            int sourceSize = (int) Offsets["SourceSize"];
            int limit = (int) Offsets["EntityCount"];
            int ptrSize = 8; // 64 bit

            byte[] characterAddressMap = process.GetByteArray(baseAddress, ptrSize * limit);
            //byte[] baseSource = process.GetByteArray(baseAddress, sourceSize);

            Dictionary<IntPtr, IntPtr> uniqueAddresses = new Dictionary<IntPtr, IntPtr>();
            IntPtr firstAddress = IntPtr.Zero;

            var firstTime = true;

            for (var i = 0; i < limit; i++) {
                IntPtr characterAddress = new IntPtr(BitConverter.TryToInt64(characterAddressMap, ptrSize * i));
                if (characterAddress == IntPtr.Zero) {
                    continue;
                }

                if (firstTime) {
                    firstAddress = characterAddress;
                    firstTime = false;
                }

                uniqueAddresses[characterAddress] = characterAddress;
            }

            // Add everyone to removed
            foreach (KeyValuePair<uint, ActorData> kvp in tempActors) {
                data.removedActors.Add(kvp.Key, new ActorData() {
                    id = kvp.Value.id,
                });
            }

            foreach (KeyValuePair<IntPtr, IntPtr> kvp in uniqueAddresses) {
                var characterAddress = new IntPtr(kvp.Value.ToInt64());
                byte[] playerSource = process.GetByteArray(characterAddress, sourceSize);

                ActorData existing = null;
                ActorData actorData = new ActorData() {
                    id = BitConverter.TryToUInt32(playerSource, Offsets["ID"]),
                };
                bool addActor = false;
                int type = playerSource[Offsets["Type"]];
                if (type == 0x01) { // Player
                    if (data.removedActors.ContainsKey(actorData.id)) {
                        data.removedActors.Remove(actorData.id);
                        tempActors.TryGetValue(actorData.id, out existing);
                    } else {
                        addActor = true;
                    }

                    // Was used for TargetID
                    //var isFirstEntry = kvp.Value.ToInt64() == firstAddress.ToInt64();

                    if (true) {
                        if (Offsets.TryGetValue("Name", out int nameid)) {
                            actorData.name = process.GetStringFromBytes(playerSource, nameid);
                        }
                        if (Offsets.TryGetValue("PerformanceID", out int perfid)) {
                            actorData.perfid = playerSource[perfid];
                        }
                    }

                    if (actorData.id != 0) {
                        if (expiringActors.ContainsKey(actorData.id)) {
                            expiringActors.Remove(actorData.id);
                        }
                    } else {
                        // Removed
                        data.addedActors.Remove(actorData.id);
                        continue;
                    }

                    // Only getting memory, no checks?
                    //EnsureMapAndZone(entry);

                    /*
                    if (isFirstEntry) {
                        if (targetAddress.ToInt64() > 0) {
                            byte[] targetInfoSource = MemoryHandler.Instance.GetByteArray(targetAddress, 128);
                            entry.TargetID = (int)BitConverter.TryToUInt32(targetInfoSource, MemoryHandler.Instance.Structures.ActorItem.ID);
                        }
                    }
                    */

                    // If removed player, just continue
                    if (existing != null) {
                        continue;
                    }
                    if (addActor) {
                        if (!tempActors.ContainsKey(actorData.id)) {
                            tempActors.Add(actorData.id, actorData);
                        }
                        data.addedActors.Add(actorData.id, actorData);
                    }

                }
            }

            // Stale removal?

            DateTime now = DateTime.Now;
            TimeSpan staleActorRemovalTime = TimeSpan.FromSeconds(0.25);
            foreach (KeyValuePair<uint, ActorData> kvp2 in data.removedActors) {
                if (!expiringActors.ContainsKey(kvp2.Key)) {
                    expiringActors[kvp2.Key] = now + staleActorRemovalTime;
                }
            }
            // check expiring list for stale actors
            foreach (KeyValuePair<uint, DateTime> kvp2 in expiringActors.ToList()) {
                if (now > kvp2.Value) {
                    tempActors.Remove(kvp2.Key);
                    expiringActors.Remove(kvp2.Key);
                } else {
                    data.removedActors.Remove(kvp2.Key);
                }
            }

            data.currentActors = tempActors;
            return data;
        }
    }
}
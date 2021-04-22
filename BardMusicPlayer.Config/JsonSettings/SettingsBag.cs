using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BardMusicPlayer.Config.JsonSettings.Inline;
using Newtonsoft.Json;

#if NET40
using ReadOnlyCollectionsExtensions.Wrappers;
#endif

namespace BardMusicPlayer.Config.JsonSettings
{
    /// <summary>
    ///     A dynamic settings class, adds settings as you go.
    /// </summary>
    /// <remarks>SettingsBag is threadsafe via accessing lock.</remarks>
    public sealed class SettingsBag : JsonSettings
    {
        private readonly SafeDictionary<string, object> _data = new SafeDictionary<string, object>();
        private readonly SafeDictionary<string, PropertyInfo> PropertyData = new SafeDictionary<string, PropertyInfo>();

        /// <summary>
        ///     All the settings in this bag.
        /// </summary>
#if NET40
        public IReadOnlyDictionary<string, object> Data => new ReadOnlyDictionaryWrapper<string, object>(_data);
#else
        public IReadOnlyDictionary<string, object> Data => _data;
#endif
        [JsonIgnore]
        public override string FileName { get; set; }

        /// <summary>
        ///     Enable autosave when a property is written.
        /// </summary>
        /// <returns></returns>
        public SettingsBag EnableAutosave()
        {
            Autosave = true;
            return this;
        }

        /// <summary>
        ///     Return a dynamic accessor that will accept any variable that can be serialized by <see cref="Newtonsoft.Json"/>.
        ///     Index access ([]) or Property/Field is working.
        /// </summary>
        /// <returns></returns>
        public dynamic AsDynamic() { return new DynamicSettingsBag(this); }

        /// <summary>
        ///     Will perform a safe after a change in any non-hardcoded public property.
        /// </summary>
        [JsonIgnore]
        public bool Autosave { get; set; } = false;

        public SettingsBag() { }

        public SettingsBag(string fileName)
        {
            FileName = fileName;
#if NET
            foreach (var pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
#else
            foreach (var pi in this.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
#endif
                if ((pi.CanRead && pi.CanWrite) == false)
                    continue;
                PropertyData.Add(pi.Name, pi);
            }
        }

        public object this[string key]
        {
            get
            {
                lock (this)
                    return Get<object>(key);
            }
            set
            {
                lock (this)
                    Set(key, value);
            }
        }

        /// <summary>
        ///     Gets the value corresponding to the given <paramref name="key"/> or returns <see cref="default(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public T Get<T>(string key, T @default = default(T))
        {
            lock (this)
            {
                if (PropertyData.ContainsKey(key))
                    return (T)PropertyData[key].GetValue(this, null);

                var ret = _data[key];
                if (ret == null || ret.Equals(default(T)))
                    return @default;

                return (T)ret;
            }
        }

        /// <summary>
        ///     Sets or adds a value.
        /// </summary>
        public void Set(string key, object value)
        {
            lock (this)
            {
                if (PropertyData.ContainsKey(key))
                    PropertyData[key].SetValue(this, value, null);
                else
                    _data[key] = value;

                if (Autosave)
                    Save();
            }
        }

        public bool Remove(string key)
        {
            bool ret = false;
            lock (this)
                ret = _data.Remove(key);
            if (Autosave)
                Save();

            return ret;
        }

        [Obsolete("Use RemoveWhere instead")]
        public int Remove(Func<KeyValuePair<string, object>, bool> comprarer) { return RemoveWhere(comprarer); }

        /// <summary>
        ///     Removes all items that <paramref name="comprarer"/> returns true to. <Br></Br>
        ///     Remove where is similar to <see cref="List{T}.RemoveAll"/>.
        /// </summary>
        public int RemoveWhere(Func<KeyValuePair<string, object>, bool> comprarer)
        {
            lock (this)
            {
                int ret = 0;
                foreach (var kv in _data.ToArray())
                {
                    if (comprarer(kv))
                        if (Remove(kv.Key))
                        {
                            ret += 1;
                        }
                }

                return ret;
            }
        }
    }
}
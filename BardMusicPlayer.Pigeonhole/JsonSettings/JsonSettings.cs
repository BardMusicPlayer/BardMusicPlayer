/*
 * Copyright(c) 2017 Eli Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.IO;
using System.Reflection;
using System.Text;
using BardMusicPlayer.Pigeonhole.JsonSettings.Inline;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BardMusicPlayer.Pigeonhole.JsonSettings
{
    public abstract class JsonSettings : ISavable, IDisposable
    {
        /// <summary>
        ///     The encoding inwhich the text will be written, by default Encoding.UTF8.
        /// </summary>
        protected static Encoding Encoding { get; set; } = Encoding.UTF8;

        protected static JsonSerializerSettings SerializationSettings { get; set; } = new() { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Include, ContractResolver = new FileNameIgnoreResolver(), TypeNameHandling = TypeNameHandling.Auto };
        
        private readonly Type _childtype;

        /// <summary>
        ///     Serves as a reminder where to save or from where to load (if it is loaded on construction and doesnt change between constructions).<br></br>
        ///     Can be relative to executing file's directory.
        /// </summary>
        [JsonIgnore]
        internal string FileName { get; set; }

        public void Save() => Save(FileName);

        /// <summary>
        ///     If this property is set, this will be used instead of the static <see cref="SerializationSettings"/>.<br></br>
        ///     Note: this property must be set during construction or as property's default value.
        /// </summary>
        protected virtual JsonSerializerSettings OverrideSerializerSettings { get; set; }

        /// <summary>
        ///     Defines how should <see cref="Load()"/> handle empty .json files, by default - false - do not throw.
        ///     Note: this property must be set during construction or as property's default value.
        /// </summary>
        protected bool ThrowOnEmptyFile { get; set; } = false;

        protected JsonSettings()
        {
            _childtype = GetType();
        }

        /// <summary>
        ///     The filename that was originally loaded from. saving to other file does not change this field!
        /// </summary>
        /// <param name="filename">the name of the file, <DEFAULT> is the default.</param>
        protected virtual void Save(string filename)
        {
            Save(_childtype, this, filename);
        }

        /// <summary>
        ///     Saves settings to a given path using custom password.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name. <br></br>Without path the file will be located at the executing directory</param>
        /// <param name="intype"></param>
        /// <param name="pSettings">The settings file to save</param>
        protected static void Save(Type intype, object pSettings, string filename)
        {
            if (pSettings is JsonSettings == false)
                throw new ArgumentException("Given param is not JsonSettings!", nameof(pSettings));
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("message", nameof(filename));

            var o = (JsonSettings)(ISavable)pSettings;
            filename = ResolvePath(filename);

            FileStream stream = null;

            try
            {
                lock (o)
                {

                    stream = Files.AttemptOpenFile(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                    o.FileName = filename;

                    var json = JsonConvert.SerializeObject(o, intype, o.OverrideSerializerSettings ?? SerializationSettings ?? JsonConvert.DefaultSettings?.Invoke());

                    var bytes = Encoding.GetBytes(json);

                    stream.SetLength(0);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    stream = null;
                }
            }
            catch (IOException e)
            {
                throw new BmpPigeonholeException("Failed writing into the file", e);
            }
            finally
            {
                stream?.Dispose();
            }
        }

        protected void Load() { Load(this, null, FileName); }
        
        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="intype">The type of this object</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        protected static object Load(Type intype, string filename) { return Load(intype.CreateInstance(), null, filename); }

        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        protected static T Load<T>(string filename) where T : ISavable { return (T)Load(typeof(T), filename); }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="instance">The instance inwhich to load into</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        protected static T Load<T>(T instance, Action configure, string filename) where T : ISavable { return (T)Load((object)instance, configure, filename); }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="instance">The instance inwhich to load into</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        protected static object Load(object instance, Action configure, string filename)
        {
            byte[] ReadAllBytes(Stream instream)
            {
                if (instream is MemoryStream stream)
                    return stream.ToArray();

                using var memoryStream = new MemoryStream();
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            JsonSettings o = (JsonSettings)(ISavable)instance;
            filename = ResolvePath(filename);
            configure?.Invoke();
            

            if (File.Exists(filename))
                try
                {
                    byte[] bytes;
                    using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                        bytes = ReadAllBytes(fs);
                    
                    var fc = Encoding.GetString(bytes);
                    if (string.IsNullOrEmpty((fc ?? "").Replace("\r", "").Replace("\n", "").Trim()))
                        if (o.ThrowOnEmptyFile)
                            throw new BmpPigeonholeException("The settings file is empty!");
                        else
                            goto _emptyfile;
                    
                    JsonConvert.PopulateObject(fc, o, o.OverrideSerializerSettings ?? SerializationSettings ?? JsonConvert.DefaultSettings?.Invoke());
                    o.FileName = filename;
                    return o;
                }
                catch (InvalidOperationException e) when (e.Message.Contains("Cannot convert"))
                {
                    throw new BmpPigeonholeException("Unable to deserialize settings file, value<->type mismatch. see inner exception", e);
                }
                catch (ArgumentException e) when (e.Message.StartsWith("Invalid"))
                {
                    throw new BmpPigeonholeException("Settings file is corrupt.");
                }

            //doesn't exist.
            _emptyfile:

            o.FileName = filename;
            o.Save(filename);

            return o;
        }

        /// <summary>
        ///     Resolves a path passed to a full absolute path.
        /// </summary>
        internal static string ResolvePath(string filename, bool throwless = false)
        {
            if (!throwless && string.IsNullOrEmpty(filename))
                throw new BmpPigeonholeException("Could not resolve path because 'FileName' is null or empty.");

            if (filename.Contains("/") || filename.Contains("\\"))
                filename = Path.Combine(Paths.NormalizePath(Path.GetDirectoryName(filename), false), Path.GetFileName(filename));
            else
                filename = Paths.CombineToExecutingBase(filename).FullName;

            return filename;
        }

        private class FileNameIgnoreResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                if (prop.PropertyName.Equals("FileName", StringComparison.OrdinalIgnoreCase))
                    prop.Ignored = true;
                return prop;
            }
        }

        private bool _isdisposed = false;

        public void Dispose()
        {
            if (_isdisposed)
                return;
            _isdisposed = true;
        }
    }
}
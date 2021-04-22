using System;
using System.IO;
using System.Reflection;
using System.Text;
using BardMusicPlayer.Config.JsonSettings.Fluent;
using BardMusicPlayer.Config.JsonSettings.Inline;
using BardMusicPlayer.Config.JsonSettings.Interface;
using BardMusicPlayer.Config.JsonSettings.Modulation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BardMusicPlayer.Config.JsonSettings
{
    #region Delegates

    public delegate void BeforeLoadHandler(ref string destinition);

    public delegate void DecryptHandler(ref byte[] data);

    public delegate void AfterDecryptHandler(ref byte[] data);

    public delegate void BeforeDeserializeHandler(ref string data);

    public delegate void AfterDeserializeHandler();

    public delegate void AfterLoadHandler();

    public delegate void BeforeSaveHandler(ref string destinition);

    public delegate void BeforeSerializeHandler();

    public delegate void AfterSerializeHandler(ref string data);

    public delegate void EncryptHandler(ref byte[] data);

    public delegate void AfterEncryptHandler(ref byte[] data);

    public delegate void AfterSaveHandler(string destinition);

    public delegate void ConfigurateHandler();

    #endregion

    public abstract class JsonSettings : ISavable, IDisposable
    {
        #region Static

        /// <summary>
        ///     The encoding inwhich the text will be written, by default Encoding.UTF8.
        /// </summary>
        public static Encoding Encoding { get; set; } = Encoding.UTF8;

        public static JsonSerializerSettings SerializationSettings { get; set; } = new JsonSerializerSettings { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Include, ContractResolver = new FileNameIgnoreResolver(), TypeNameHandling = TypeNameHandling.Auto };

        #endregion

        private readonly Type _childtype;

        /// <summary>
        ///     Serves as a reminder where to save or from where to load (if it is loaded on construction and doesnt change between constructions).<br></br>
        ///     Can be relative to executing file's directory.
        /// </summary>
        [JsonIgnore]
        public abstract string FileName { get; set; }

        #region Modularity

        /// <summary>
        ///     Modulation Manager, handles everything related to modules in this instance.
        /// </summary>
        [JsonIgnore]
        public ModuleSocket Modulation { get; }

        #endregion

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
            Modulation = new ModuleSocket(this);
            _childtype = GetType();
            if (!_childtype.HasDefaultConstructor())
                throw new JsonSettingsException($"Can't initiate a settings object with class that doesn't have empty public constructor.");
        }

        protected JsonSettings(string fileName) : this() { FileName = fileName; }


        #region Loading & Saving

        #region Save

        /// <summary>
        ///     The filename that was originally loaded from. saving to other file does not change this field!
        /// </summary>
        /// <param name="filename">the name of the file, <DEFAULT> is the default.</param>
        public virtual void Save(string filename)
        {
            Save(_childtype, this, filename);
            //File.WriteAllText(filename, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        ///     Save the settings file to a predefined location <see cref="ISavable.FileName" />
        /// </summary>
        public void Save() { Save("<DEFAULT>"); }


        /// <summary>
        ///     Saves settings to a given path using custom password.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name. <br></br>Without path the file will be located at the executing directory</param>
        /// <param name="intype"></param>
        /// <param name="pSettings">The settings file to save</param>
        public static void Save(Type intype, object pSettings, string filename = "<DEFAULT>")
        {
            if (pSettings is JsonSettings == false)
                throw new ArgumentException("Given param is not JsonSettings!", nameof(pSettings));
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("message", nameof(filename));

            var o = (JsonSettings)pSettings;
            filename = ResolvePath(o, filename);
            o.EnsureConfigured();
            FileStream stream = null;

            try
            {
                lock (o)
                {
                    o.OnBeforeSave(ref filename);
                    stream = Files.AttemptOpenFile(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                    o.FileName = filename;
                    o.OnBeforeSerialize();
                    var json = JsonConvert.SerializeObject(o, intype, o.OverrideSerializerSettings ?? SerializationSettings ?? JsonConvert.DefaultSettings?.Invoke());
                    o.OnAfterSerialize(ref json);
                    var bytes = Encoding.GetBytes(json);
                    o.OnEncrypt(ref bytes);
                    o.OnAfterEncrypt(ref bytes);

                    stream.SetLength(0);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    stream = null;

                    o.OnAfterSave(filename);
                }
            }
            catch (IOException e)
            {
                throw new JsonSettingsException("Failed writing into the file", e);
            }
            finally
            {
                stream?.Dispose();
            }
        }

        /// <summary>
        ///     Saves settings to a given path using custom password.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name. <br></br>Without path the file will be located at the executing directory</param>
        /// <param name="pSettings">The settings file to save</param>
        public static void Save<T>(T pSettings, string filename = "<DEFAULT>") where T : ISavable { Save(typeof(T), pSettings, filename); }

        #endregion

        #region Load

        #region Regular Load

        public void Load() { Load(this, (Action)null, FileName); }

        public void Load(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            Load(this, (Action)null, filename);
        }

        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public void Load(string filename, Action<JsonSettings> configure)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            Load(this, configure, filename);
        }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="intype">The type of this object</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static object Load(Type intype, string filename = "<DEFAULT>") { return Load(intype.CreateInstance(), (Action)null, filename); }

        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(string filename = "<DEFAULT>") where T : ISavable { return (T)Load(typeof(T), filename); }


        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="intype">The type of this object</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static object Load<T>(Type intype, Action<T> configure, string filename = "<DEFAULT>") where T : ISavable { return Load((T)intype.CreateInstance(), configure, filename); }

        #endregion

        #region Load With Args

        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(Action<T> configure, string filename, object[] args) where T : ISavable { return (T)Load(typeof(T), configure, filename); }

        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(string filename, Action<T> configure, object[] args) where T : ISavable
        {
            T o = (T)typeof(T).CreateInstance(args);
            return Load(o, () => configure?.Invoke(o), filename);
        }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="intype">The type of this object</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static object Load(Type intype, object[] args) { return Load(intype, null, "<DEFAULT>", args); }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="intype">The type of this object</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static object Load(Type intype, string filename, object[] args) { return Load(intype, null, filename, args); }

        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(object[] args) where T : ISavable { return (T)Load(typeof(T), args); }


        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(Action<T> configure, string filename = "<DEFAULT>") where T : ISavable { return (T)Load(typeof(T), configure, filename); }

        /// <summary>
        ///     Loads or creates a settings file.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(string filename, Action<T> configure) where T : ISavable { return (T)Load(typeof(T), configure, filename); }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="intype">The type of this object</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static object Load(Type intype, Action configure, string filename, object[] args) { return Load(intype.CreateInstance(args), configure, filename); }

        #endregion

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="instance">The instance inwhich to load into</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(T instance, Action<T> configure, string filename = "<DEFAULT>") where T : ISavable { return Load(instance, () => configure?.Invoke(instance), filename); }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="instance">The instance inwhich to load into</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static T Load<T>(T instance, Action configure, string filename = "<DEFAULT>") where T : ISavable { return (T)Load((object)instance, configure, filename); }

        /// <summary>
        ///     Loads a settings file or creates a new settings file.
        /// </summary>
        /// <param name="instance">The instance inwhich to load into</param>
        /// <param name="configure">Configurate the settings instance prior to loading - called after OnConfigure</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>The loaded or freshly new saved object</returns>
        public static object Load(object instance, Action configure, string filename = "<DEFAULT>")
        {
            byte[] ReadAllBytes(Stream instream)
            {
                if (instream is MemoryStream stream)
                    return stream.ToArray();

                using (var memoryStream = new MemoryStream())
                {
                    instream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            JsonSettings o = (JsonSettings)(ISavable)instance;
            filename = ResolvePath(o, filename);
            o.EnsureConfigured();
            configure?.Invoke();

            o.OnBeforeLoad(ref filename);

            if (File.Exists(filename))
                try
                {
                    byte[] bytes;
                    using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                        bytes = ReadAllBytes(fs);

                    o.OnDecrypt(ref bytes);
                    o.OnAfterDecrypt(ref bytes);

                    var fc = Encoding.GetString(bytes);
                    if (string.IsNullOrEmpty((fc ?? "").Replace("\r", "").Replace("\n", "").Trim()))
                        if (o.ThrowOnEmptyFile)
                            throw new JsonSettingsException("The settings file is empty!");
                        else
                            goto _emptyfile;

                    o.OnBeforeDeserialize(ref fc);
                    JsonConvert.PopulateObject(fc, o, o.OverrideSerializerSettings ?? SerializationSettings ?? JsonConvert.DefaultSettings?.Invoke());
                    o.OnAfterDeserialize();
                    o.FileName = filename;
                    o.OnAfterLoad();
                    return o;
                }
                catch (InvalidOperationException e) when (e.Message.Contains("Cannot convert"))
                {
                    throw new JsonSettingsException("Unable to deserialize settings file, value<->type mismatch. see inner exception", e);
                }
                catch (ArgumentException e) when (e.Message.StartsWith("Invalid"))
                {
                    throw new JsonSettingsException("Settings file is corrupt.");
                }

            //doesn't exist.
            _emptyfile:
            o.OnAfterLoad();
            o.FileName = filename;
            o.Save(filename);

            return o;
        }

        #endregion

        #region Configure

        /// <summary>
        ///     Create a settings object for further configuration.
        /// </summary>
        /// <param name="intype">The type of the configuration file.</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>A freshly new object or <paramref name="intype"/>.</returns>
        public static object Configure(Type intype, string filename = "<DEFAULT>") { return Configure((ISavable)intype.CreateInstance(), filename); }

        /// <summary>
        ///     Create a settings object for further configuration.
        /// </summary>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        public static T Configure<T>(string filename = "<DEFAULT>") where T : ISavable { return (T)Configure(typeof(T), filename); }

        /// <summary>
        ///     Create a settings object for further configuration.
        /// </summary>
        /// <param name="instance">An instance if available.</param>
        /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.<br></br>Without path the file will be located at the executing directory</param>
        /// <returns>A freshly new object or <paramref name="instance"/>.</returns>
        public static T Configure<T>(T instance, string filename = "<DEFAULT>") where T : ISavable
        {
            JsonSettings o = (JsonSettings)((ISavable)instance ?? (T)typeof(T).CreateInstance());
            FluentJsonSettings._withFileName(o, filename, true);
            o.EnsureConfigured();
            return (T)(object)o;
        }

        #endregion

        #region Construct

        /// <summary>
        ///     Constucts a settings object for further configuration.
        /// </summary>
        /// <param name="args">The arguments that will be passed into the constructor.</param>
        /// <returns>A freshly new object.</returns>
        public static T Construct<T>(params object[] args) where T : ISavable
        {
            JsonSettings o = (JsonSettings)(ISavable)Activator.CreateInstance(typeof(T), args);
            o.EnsureConfigured();
            return (T)(object)o;
        }

        #endregion

        /// <summary>
        ///     Resolves a path passed to a full absolute path.
        /// </summary>
        /// <remarks>This overload handles default value of passed <see cref="o"/></remarks>
        internal static string ResolvePath<T>(T o, string filename, bool throwless = false) where T : JsonSettings
        {
            if (!throwless && (string.IsNullOrEmpty(filename) || (filename == "<DEFAULT>" && string.IsNullOrEmpty(o.FileName))))
                throw new JsonSettingsException("Could not resolve path because 'FileName' is null or empty.");

            if (filename == "<DEFAULT>")
            {
                if (o.FileName == null) //param filename is default and o.FileName are null.
                    return null;
                filename = o.FileName; //load from instance.
            }



            return ResolvePath(filename, throwless);
        }

        /// <summary>
        ///     Resolves a path passed to a full absolute path.
        /// </summary>
        internal static string ResolvePath(string filename, bool throwless = false)
        {
            if (!throwless && string.IsNullOrEmpty(filename))
                throw new JsonSettingsException("Could not resolve path because 'FileName' is null or empty.");

            if (filename.Contains("/") || filename.Contains("\\"))
                filename = Path.Combine(Paths.NormalizePath(Path.GetDirectoryName(filename), false), Path.GetFileName(filename));
            else
                filename = Paths.CombineToExecutingBase(filename).FullName;

            return filename;
        }

        #endregion

        #region Events

        #region Inheritable Events

        public event BeforeLoadHandler BeforeLoad;

        private event DecryptHandler _decrypt;

        //reverse insert
        public event DecryptHandler Decrypt
        {
            add => this._decrypt = value + _decrypt;
            remove => this._decrypt -= value;
        }

        public virtual event AfterDecryptHandler AfterDecrypt;

        public virtual event BeforeDeserializeHandler BeforeDeserialize;

        public virtual event AfterDeserializeHandler AfterDeserialize;

        public virtual event AfterLoadHandler AfterLoad;

        public virtual event BeforeSaveHandler BeforeSave;

        public virtual event BeforeSerializeHandler BeforeSerialize;

        public virtual event AfterSerializeHandler AfterSerialize;

        public virtual event EncryptHandler Encrypt;

        public virtual event AfterEncryptHandler AfterEncrypt;

        public virtual event AfterSaveHandler AfterSave;

        public virtual event ConfigurateHandler Configurate;

        #endregion

        private bool _hasconfigured = false;

        /// <summary>
        ///     Configurate properties of this JsonSettings, for example - call <see cref="FluentJsonSettings.WithBase64{T}"/> on this.<br></br>
        /// </summary>
        protected virtual void OnConfigure()
        {
            if (_hasconfigured)
                throw new InvalidOperationException("Can't run configure twice!");
            _hasconfigured = true;
            Configurate?.Invoke();
        }

        protected internal void EnsureConfigured()
        {
            if (_hasconfigured)
                return;
            OnConfigure();
        }

        internal virtual void OnBeforeLoad(ref string destinition) { BeforeLoad?.Invoke(ref destinition); }

        public virtual void OnDecrypt(ref byte[] data) { _decrypt?.Invoke(ref data); }

        internal virtual void OnAfterDecrypt(ref byte[] data) { AfterDecrypt?.Invoke(ref data); }

        internal virtual void OnBeforeDeserialize(ref string data) { BeforeDeserialize?.Invoke(ref data); }

        internal virtual void OnAfterDeserialize() { AfterDeserialize?.Invoke(); }

        internal virtual void OnAfterLoad() { AfterLoad?.Invoke(); }

        internal virtual void OnBeforeSave(ref string destinition) { BeforeSave?.Invoke(ref destinition); }

        internal virtual void OnBeforeSerialize() { BeforeSerialize?.Invoke(); }

        internal virtual void OnAfterSerialize(ref string data) { AfterSerialize?.Invoke(ref data); }

        public virtual void OnEncrypt(ref byte[] data) { Encrypt?.Invoke(ref data); }

        internal virtual void OnAfterEncrypt(ref byte[] data) { AfterEncrypt?.Invoke(ref data); }

        internal virtual void OnAfterSave(string destinition) { AfterSave?.Invoke(destinition); }

        #endregion

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
            Modulation.Dispose();
        }
    }
}
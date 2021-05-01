/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

namespace BardMusicPlayer.Config.JsonSettings.Interface
{
    public interface ISavable
    {
        /// <summary>
        ///     The path for this ISaveable file, relative pathing allowed to current executing file.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        ///     Save the settings file to a specific location!
        /// </summary>
        /// <param name="filename"></param>
        void Save(string filename);

        /// <summary>
        ///     Save the settings file to a predefined location and name <see cref="FileName" />
        /// </summary>
        void Save();

        /// <summary>
        ///     Populate the data in this object from <see cref="FileName"/>.
        /// </summary>
        void Load();

        /// <summary>
        ///     Populate the data in this object from given <paramref name="filename"/>.
        /// </summary>
        /// <param name="filename">The path to the file inwhich to load, relative pathing allowed to current executing file.</param>
        void Load(string filename);

        #region Loading

        /// <summary>
        ///     Invoked after path has been resolved and before reading. <br></br>
        ///     FileInfo can be modified now.
        /// </summary>
        event BeforeLoadHandler BeforeLoad;
        /// <summary>
        ///     Called during loading right after <see cref="BeforeLoad"/> to decrypt the readed bytes, if <see cref="Encrypt"/> is not implemented - no reason to perform decryption.
        /// </summary>
        /// <param name="data">The data that was read from the file.</param>
        event DecryptHandler Decrypt;
        /// <summary>
        ///     Called after <see cref="Decrypt"/>.
        /// </summary>
        /// <param name="data"></param>
        event AfterDecryptHandler AfterDecrypt;

        /// <summary>
        ///     Invoked after file was read and decrypted successfully right before deserializing into an object.
        /// </summary>
        event BeforeDeserializeHandler BeforeDeserialize;

        /// <summary>
        ///     Invoked after deserialization of <see cref="this"/> was successful.
        /// </summary>
        event AfterDeserializeHandler AfterDeserialize;


        /// <summary>
        ///     Invoked at the end of the loading progress.
        /// </summary>
        event AfterLoadHandler AfterLoad;
        #endregion

        #region Saving

        /// <summary>
        ///     Invoked before saving this object.
        /// </summary>
        event BeforeSaveHandler BeforeSave;

        event BeforeSerializeHandler BeforeSerialize;

        event AfterSerializeHandler AfterSerialize;

        /// <summary>
        ///     After serializing, encryption can be applied now.
        /// </summary>
        event EncryptHandler Encrypt;

        /// <summary>
        ///     After encryption successful.
        /// </summary>
        event AfterEncryptHandler AfterEncrypt;

        /// <summary>
        ///     Invoked after saving this object.
        /// </summary>
        event AfterSaveHandler AfterSave;

        #endregion
    }
}
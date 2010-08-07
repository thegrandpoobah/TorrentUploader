using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cleverscape.UTorrentClient.WebClient
{
    #region SettingBase

    /// <summary>
    /// Represents a uTorrent setting
    /// </summary>
    public abstract class SettingBase
    {
        protected UTorrentWebClient _parentWebClient;

        #region Constructor and parsing data from Web UI

        internal SettingBase(UTorrentWebClient ParentWebClient)
        {
            _parentWebClient = ParentWebClient;
            Name = "";
            Type = SettingType.String;
        }

        internal static SettingType Parse(string Type)
        {
            switch (Type)
            {
                case "0":
                    return SettingType.Integer;
                case "1":
                    return SettingType.Boolean;
                case "2":
                    return SettingType.String;
                default:
                    return SettingType.String;
            }
        }

        #endregion

        /// <summary>
        /// The name of the setting
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the setting
        /// </summary>
        public SettingType Type { get; set; }

        #region Static Methods

        internal static SettingBase CreateSetting(SettingType Type, string Name, UTorrentWebClient ParentWebClient)
        {
            switch (Type)
            {
                case SettingType.Integer:
                    return new SettingInteger(ParentWebClient) { Name = Name };
                case SettingType.String:
                    return new SettingString(ParentWebClient) { Name = Name };
                case SettingType.Boolean:
                    return new SettingBoolean(ParentWebClient) { Name = Name };
                default:
                    return new SettingString(ParentWebClient) { Name = Name };
            }
        }

        internal static void SetValue(SettingBase Setting, string Value)
        {
            switch (Setting.Type)
            {
                case SettingType.Integer:
                    ((SettingInteger)Setting).ValueInternal = int.Parse(Value);
                    break;
                case SettingType.String:
                    ((SettingString)Setting).ValueInternal = Value;
                    break;
                case SettingType.Boolean:
                    ((SettingBoolean)Setting).ValueInternal = Value.ToLowerInvariant() == "true";
                    break;
                default:
                    ((SettingString)Setting).ValueInternal = Value;
                    break;
            }
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            return (obj is SettingBase) && ((SettingBase)obj).Name == this.Name && ((SettingBase)obj).Type == this.Type;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name.ToString();
        }

        #endregion
    }

    #endregion

    #region SettingBase<T>

    /// <summary>
    /// Represents a uTorrent setting specialised to storing a value of type T
    /// </summary>
    /// <typeparam name="T">The type of the setting being stored</typeparam>
    public abstract class SettingBase<T> : SettingBase
    {
        internal SettingBase(UTorrentWebClient ParentWebClient) : base(ParentWebClient) { }

        protected T _internalValue;

        /// <summary>
        /// The value of the setting.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of settings.
        /// </summary>
        public T Value
        {
            get
            {
                return _internalValue;
            }
            set
            {
                _internalValue = value;
                _parentWebClient.SetSetting(this);
            }
        }

        internal T ValueInternal
        {
            get
            {
                return _internalValue;
            }
            set
            {
                _internalValue = value;
            }
        }
    }

    #endregion

    #region SettingInteger

    /// <summary>
    /// Represents a uTorrent setting that takes integer values 
    /// </summary>
    public class SettingInteger : SettingBase<int>
    {
        internal SettingInteger(UTorrentWebClient ParentWebClient)
            : base(ParentWebClient)
        {
            Type = SettingType.Integer;
            _internalValue = 0;
        }
    }

    #endregion

    #region SettingString

    /// <summary>
    /// Represents a uTorrent setting that takes string values 
    /// </summary>
    public class SettingString : SettingBase<string>
    {
        internal SettingString(UTorrentWebClient ParentWebClient)
            : base(ParentWebClient)
        {
            Type = SettingType.String;
            _internalValue = "";
        }
    }

    #endregion

    #region SettingBoolean

    /// <summary>
    /// Represents a uTorrent setting that takes boolean values 
    /// </summary>
    public class SettingBoolean : SettingBase<bool>
    {
        internal SettingBoolean(UTorrentWebClient ParentWebClient)
            : base(ParentWebClient)
        {
            Type = SettingType.Boolean;
            _internalValue = false;
        }
    }

    #endregion

    #region SettingType enum

    /// <summary>
    /// The type of the value of a setting
    /// </summary>
    public enum SettingType
    {
        /// <summary>
        /// An integer value
        /// </summary>
        Integer,
        /// <summary>
        /// A string value
        /// </summary>
        String,
        /// <summary>
        /// A boolean value
        /// </summary>
        Boolean
    }

    #endregion

    /// <summary>
    /// Represents a collection of uTorrent settings 
    /// </summary>
    public class SettingsCollection : IEnumerable<SettingBase>
    {
        private List<SettingBase> _settingsCollectionInternal;

        #region Constructor and parsing data from Web UI

        internal SettingsCollection()
        {
            _settingsCollectionInternal = new List<SettingBase>();
        }

        internal void ParseSettings(List<string[]> Settings, UTorrentWebClient ParentWebClient)
        {
            foreach (string[] SettingArray in Settings)
            {
                if (SettingArray.Length != 3)
                {
                    throw new FormatException("The array of setting data was not in the expected format.");
                }
                SettingType CurrentType = SettingBase.Parse(SettingArray[1]);
                SettingBase NewSetting = this.GetByNameAndType(SettingArray[0], CurrentType);
                if (NewSetting == null)
                {
                    NewSetting = SettingBase.CreateSetting(CurrentType, SettingArray[0], ParentWebClient);
                    SettingBase.SetValue(NewSetting, SettingArray[2]);
                    _settingsCollectionInternal.Add(NewSetting);
                }
                else
                {
                    SettingBase.SetValue(NewSetting, SettingArray[2]);
                }
                // Note: this assumes settings will never be removed from the list that uTorrent returns. 
                // This assumption should be true unless the server (i.e. uTorrent) is upgraded or changed whilst the client is active.
            }
        }

        #endregion

        #region Public Methods

        public bool Contains(string Name, SettingType Type)
        {
            foreach (SettingBase CurrentSetting in _settingsCollectionInternal)
            {
                if (CurrentSetting.Name == Name && CurrentSetting.Type == Type)
                {
                    return true;
                }
            }
            return false;
        }

        public SettingBase GetByNameAndType(string Name, SettingType Type)
        {
            foreach (SettingBase CurrentSetting in _settingsCollectionInternal)
            {
                if (CurrentSetting.Name == Name && CurrentSetting.Type == Type)
                {
                    return CurrentSetting;
                }
            }
            return null;
        }

        public IEnumerator<SettingBase> GetEnumerator()
        {
            return _settingsCollectionInternal.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _settingsCollectionInternal.GetEnumerator();
        }

        public int IndexOf(SettingBase item)
        {
            return _settingsCollectionInternal.IndexOf(item);
        }

        public SettingBase this[int index]
        {
            get
            {
                return _settingsCollectionInternal[index];
            }
        }

        public bool Contains(SettingBase item)
        {
            return Contains(item.Name, item.Type);
        }

        public void CopyTo(SettingBase[] array, int arrayIndex)
        {
            _settingsCollectionInternal.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _settingsCollectionInternal.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        #endregion
    }
}

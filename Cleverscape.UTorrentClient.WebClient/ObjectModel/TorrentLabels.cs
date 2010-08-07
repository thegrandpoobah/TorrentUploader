using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cleverscape.UTorrentClient.WebClient.ServiceDefinition;

namespace Cleverscape.UTorrentClient.WebClient
{
    /// <summary>
    /// Represents a torrent label
    /// </summary>
    public class TorrentLabel
    {
        #region Constructors and parsing data from Web UI
        
        internal TorrentLabel(string[] InputValues)
        {
            SetValuesFromStringArray(InputValues);
        }

        internal void SetValuesFromStringArray(string[] InputValues)
        {
            if (InputValues.Length != 2)
            {
                throw new FormatException("The array of label data was not in the expected format.");
            }
            Label = InputValues[0];
            NumberOfTorrentsInLabel = int.Parse(InputValues[1]);
        }

        #endregion

        /// <summary>
        /// The label text for this label
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// The number of torrents currently in this label
        /// </summary>
        public int NumberOfTorrentsInLabel { get; private set; }

        #region Public Methods

        public override bool Equals(object obj)
        {
            return (obj is TorrentLabel) && ((TorrentLabel)obj).Label == this.Label;
        }

        public override int GetHashCode()
        {
            return this.Label.GetHashCode();
        }

        public override string ToString()
        {
            return this.Label;
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of torrent labels
    /// </summary>
    public class TorrentLabelCollection : IEnumerable<TorrentLabel>
    {
        private List<TorrentLabel> _torrentLabelCollectionInternal;

        #region Constructor and parsing data from Web UI

        internal TorrentLabelCollection()
        {
            _torrentLabelCollectionInternal = new List<TorrentLabel>();
        }

        internal void Parse(LabelsList CurrentLabels)
        {
            List<string> ListOfLabels = new List<string>();
            foreach (string[] LabelArray in CurrentLabels)
            {
                if (LabelArray.Length == 0)
                {
                    throw new FormatException("The array of label data was not in the expected format.");
                }
                ListOfLabels.Add(LabelArray[0]);
                TorrentLabel NewTorrentLabel = GetByLabel(LabelArray[0]);
                if (NewTorrentLabel == null)
                {
                    NewTorrentLabel = new TorrentLabel(LabelArray);
                    _torrentLabelCollectionInternal.Add(NewTorrentLabel);
                }
                else
                {
                    NewTorrentLabel.SetValuesFromStringArray(LabelArray);
                }
            }
            RemoveWhereLabelCodeIsNotInList(ListOfLabels);
        }

        #endregion

        #region Private Methods

        private void RemoveByLabel(string Label)
        {
            List<TorrentLabel> TorrentLabelsToRemove = new List<TorrentLabel>();
            foreach (TorrentLabel CurrentTorrentLabel in _torrentLabelCollectionInternal)
            {
                if (CurrentTorrentLabel.Label == Label)
                {
                    TorrentLabelsToRemove.Add(CurrentTorrentLabel);
                }
            }
            foreach (TorrentLabel TorrentLabelToRemove in TorrentLabelsToRemove)
            {
                _torrentLabelCollectionInternal.Remove(TorrentLabelToRemove);
            }
        }

        private void RemoveWhereLabelCodeIsNotInList(IEnumerable<string> ListOfLabels)
        {
            List<TorrentLabel> TorrentLabelsToRemove = new List<TorrentLabel>();
            foreach (TorrentLabel CurrentTorrentLabel in _torrentLabelCollectionInternal)
            {
                if (!ListOfLabels.Contains(CurrentTorrentLabel.Label))
                {
                    TorrentLabelsToRemove.Add(CurrentTorrentLabel);
                }
            }
            foreach (TorrentLabel TorrentLabelToRemove in TorrentLabelsToRemove)
            {
                _torrentLabelCollectionInternal.Remove(TorrentLabelToRemove);
            }
        }

        #endregion

        #region Public Methods

        public bool ContainsLabel(string Label)
        {
            return (GetByLabel(Label) != null);
        }

        public TorrentLabel GetByLabel(string Label)
        {
            foreach (TorrentLabel CurrentTorrentLabel in _torrentLabelCollectionInternal)
            {
                if (CurrentTorrentLabel.Label == Label)
                {
                    return CurrentTorrentLabel;
                }
            }
            return null;
        }

        public int IndexOf(TorrentLabel item)
        {
            return _torrentLabelCollectionInternal.IndexOf(item);
        }

        public TorrentLabel this[int index]
        {
            get
            {
                return _torrentLabelCollectionInternal[index];
            }
        }

        public bool Contains(TorrentLabel item)
        {
            return (GetByLabel(item.Label) != null);
        }

        public void CopyTo(TorrentLabel[] array, int arrayIndex)
        {
            _torrentLabelCollectionInternal.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _torrentLabelCollectionInternal.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerator<TorrentLabel> GetEnumerator()
        {
            return _torrentLabelCollectionInternal.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _torrentLabelCollectionInternal.GetEnumerator();
        }

        #endregion
    }
}

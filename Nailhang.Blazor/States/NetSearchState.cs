using Nailhang.Display.NetPublicSearch.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Blazor.States
{
    public class NetSearchState
    {
        public string[] Namespaces { get; set; }

        string selectedNamespace;
        private string query;
        private bool showOnlyPublic;

        public string SelectedNamespace
        {
            get => selectedNamespace;
            set
            {
                if (selectedNamespace == value)
                    return;

                selectedNamespace = value;
                if (SelectedNamespaceChanged != null)
                    SelectedNamespaceChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler SelectedNamespaceChanged;

        public bool ShowOnlyPublic 
        {
            get => showOnlyPublic;
            set
            {
                if (showOnlyPublic == value)
                    return;
                showOnlyPublic = value;
                if (ShowOnlyPublicChanged != null)
                    ShowOnlyPublicChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler ShowOnlyPublicChanged;

        public ISearchItem[] DisplayItems { get; set; }

        public string Query
        {
            get => query;
            set
            {
                if (query == value)
                    return;

                query = value;
                if (QueryChanged != null)
                    QueryChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler QueryChanged;
    }
}

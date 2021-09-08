﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using WDE.Common;
using WDE.Common.Database;
using WDE.SmartScriptEditor.Models;

namespace WDE.SmartScriptEditor
{
    public class SmartScriptSolutionItem : ISolutionItem, ISmartScriptSolutionItem
    {
        public SmartScriptSolutionItem(int entry, SmartScriptType smartType)
        {
            Entry = entry;
            SmartType = smartType;
        }

        public int Entry { get; }
        public SmartScriptType SmartType { get; }
        
        public void UpdateDependants(HashSet<long> usedTimed)
        {
            for (int i = Items.Count - 1; i >= 0; --i)
            {
                if (!usedTimed.Contains(((SmartScriptSolutionItem) Items[i]).Entry))
                    Items.RemoveAt(i);
                else
                    usedTimed.Remove(((SmartScriptSolutionItem) Items[i]).Entry);
            }

            foreach (var t in usedTimed)
            {
                Items.Add(new SmartScriptSolutionItem((int)t, SmartScriptType.TimedActionList));
            }

        }

        [JsonIgnore]
        public bool IsContainer { get; set; }

        [JsonIgnore] 
        public ObservableCollection<ISolutionItem> Items { get; set; } = new();

        public string? ExtraId => SmartType is SmartScriptType.AreaTrigger or SmartScriptType.TimedActionList ? null : Entry.ToString();

        [JsonIgnore]
        public bool IsExportable => true;

        public ISolutionItem Clone() => new SmartScriptSolutionItem(Entry, SmartType);

        protected bool Equals(SmartScriptSolutionItem other)
        {
            return Entry == other.Entry && SmartType == other.SmartType;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SmartScriptSolutionItem)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Entry, (int)SmartType);
        }
    }
}
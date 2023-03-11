using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using WDE.Common;
using WDE.Common.Avalonia.Controls;
using WDE.Common.Collections;
using WDE.Common.DBC;
using WDE.Common.Services;
using WDE.Common.TableData;
using WDE.Common.Utils;
using WDE.Module.Attributes;

namespace WoWDatabaseEditorCore.Avalonia.Services.EntrySelectorService;

[AutoRegister]
[SingleInstance]
public class SpellEntryProviderService : ISpellEntryProviderService
{
    private readonly ITabularDataPicker tabularDataPicker;
    private readonly ISpellStore spellStore;

    public SpellEntryProviderService(ITabularDataPicker tabularDataPicker,
        ISpellStore spellStore)
    {
        this.tabularDataPicker = tabularDataPicker;
        this.spellStore = spellStore;
    }

    public async Task<uint?> GetEntryFromService(uint? spellId = null)
    {
        var index = -1;
        var spells = spellStore.Spells;
        
        if (spellId.HasValue)
        {
            for (int i = 0, count = spells.Count; i < count; ++i)
                if (spells[i].Id == spellId)
                {
                    index = i;
                    break;
                }
        }
        var result = await tabularDataPicker.PickRow(new TabularDataBuilder<ISpellEntry>()
            .SetTitle("Pick a spell")
            .SetData(spells.AsIndexedCollection())
            .SetColumns(new TabularDataColumn(nameof(ISpellEntry.Id), "Entry", 60),
                new TabularDataColumn(nameof(ISpellEntry.Id), "Icon", 40, new FuncDataTemplate(_ => true,
                    (_, _) => new SpellImage()
                    {
                        [!SpellImage.SpellIdProperty] = new Binding(nameof(ISpellEntry.Id)),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(2)
                    })),
                new TabularDataColumn(nameof(ISpellEntry.Name), "Name", 300),
                new TabularDataColumn(nameof(ISpellEntry.Aura), "Aura", 100),
                new TabularDataColumn(nameof(ISpellEntry.Targets), "Targets", 130))
            .SetFilter((entry, text) => entry.Id.Contains(text) || entry.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
            .Build(), index);
        return result?.Id;
    }

}
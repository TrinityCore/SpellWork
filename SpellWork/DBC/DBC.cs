using DBFileReaderLib;
using SpellWork.DBC.Structures;
using SpellWork.Extensions;
using SpellWork.GameTables;
using SpellWork.GameTables.Structures;
using SpellWork.Properties;
using SpellWork.Spell;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SpellWork.Database;
using SpellWork.Utilities;

namespace SpellWork.DBC
{
    public static class DBC
    {
        public const string Version = "SpellWork 10.1.0 (49318)";
        public const uint MaxLevel = 70;
        public const uint MaxItemLevel = 1300;

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable CollectionNeverUpdated.Global
        public static Storage<AreaGroupMemberEntry>             AreaGroupMember { get; set; }
        public static Storage<AreaTableEntry>                   AreaTable { get; set; }
        public static Storage<ContentTuningEntry>               ContentTuning { get; set; }
        public static Storage<ContentTuningXExpectedEntry>      ContentTuningXExpected { get; set; }
        public static Storage<DifficultyEntry>                  Difficulty { get; set; }
        public static Storage<ExpectedStatEntry>                ExpectedStat { get; set; }
        public static Storage<ExpectedStatModEntry>             ExpectedStatMod { get; set; }
        public static Storage<MapEntry>                         Map { get; set; }
        public static Storage<MapDifficultyEntry>               MapDifficulty { get; set; }
        public static Storage<OverrideSpellDataEntry>           OverrideSpellData { get; set; }
        public static Storage<ScreenEffectEntry>                ScreenEffect { get; set; }
        public static Storage<SpellEntry>                       Spell { get; set; }
        public static Storage<SpellNameEntry>                   SpellName { get; set; }
        public static Storage<SpellAuraOptionsEntry>            SpellAuraOptions { get; set; }
        public static Storage<SpellAuraRestrictionsEntry>       SpellAuraRestrictions { get; set; }
        public static Storage<SpellCastingRequirementsEntry>    SpellCastingRequirements { get; set; }
        public static Storage<SpellCastTimesEntry>              SpellCastTimes { get; set; }
        public static Storage<SpellCategoriesEntry>             SpellCategories { get; set; }
        public static Storage<SpellCategoryEntry>               SpellCategory { get; set; }
        public static Storage<SpellClassOptionsEntry>           SpellClassOptions { get; set; }
        public static Storage<SpellCooldownsEntry>              SpellCooldowns { get; set; }
        public static Storage<SpellDescriptionVariablesEntry>   SpellDescriptionVariables { get; set; }
        public static Storage<SpellDurationEntry>               SpellDuration { get; set; }
        public static Storage<SpellEffectEntry>                 SpellEffect { get; set; }
        public static Storage<SpellMiscEntry>                   SpellMisc { get; set; }
        public static Storage<SpellEquippedItemsEntry>          SpellEquippedItems { get; set; }
        public static Storage<SpellInterruptsEntry>             SpellInterrupts { get; set; }
        public static Storage<SpellLabelEntry>                  SpellLabel { get; set; }
        public static Storage<SpellLevelsEntry>                 SpellLevels { get; set; }
        public static Storage<SpellPowerEntry>                  SpellPower { get; set; }
        public static Storage<SpellRadiusEntry>                 SpellRadius { get; set; }
        public static Storage<SpellRangeEntry>                  SpellRange { get; set; }
        public static Storage<SpellScalingEntry>                SpellScaling { get; set; }
        public static Storage<SpellShapeshiftEntry>             SpellShapeshift { get; set; }
        public static Storage<SpellTargetRestrictionsEntry>     SpellTargetRestrictions { get; set; }
        public static Storage<SpellTotemsEntry>                 SpellTotems { get; set; }
        public static Storage<SpellXDescriptionVariablesEntry>  SpellXDescriptionVariables { get; set; }
        public static Storage<SpellXSpellVisualEntry>           SpellXSpellVisual { get; set; }
        public static Storage<RandPropPointsEntry>              RandPropPoints { get; set; }
        public static Storage<SpellProcsPerMinuteEntry>         SpellProcsPerMinute { get; set; }

        public static Storage<SkillLineAbilityEntry>            SkillLineAbility { get; set; }
        public static Storage<SkillLineEntry>                   SkillLine { get; set; }

        public static Storage<ItemEntry>                        Item { get; set; }
        public static Storage<ItemEffectEntry>                  ItemEffect { get; set; }
        public static Storage<ItemSparseEntry>                  ItemSparse { get; set; }
        public static Storage<ItemXItemEffectEntry>             ItemXItemEffect { get; set; }

        public static Storage<SpellReagentsEntry>               SpellReagents { get; set; }
        public static Storage<SpellReagentsCurrencyEntry>       SpellReagentsCurrency { get; set; }
        public static Storage<SpellMissileEntry>                SpellMissile { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore CollectionNeverUpdated.Global

        // public static Storage<SpellMissileMotionEntry>         SpellMissileMotion { get; public set; }
        // public static Storage<SpellVisualEntry>                SpellVisual { get; public set; }

        public static readonly IDictionary<int, SpellInfo> SpellInfoStore = new ConcurrentDictionary<int, SpellInfo>();
        public static readonly IDictionary<int, ISet<int>> SpellTriggerStore = new Dictionary<int, ISet<int>>();

        const int ProgressStoresSteps = 21;

        private enum Progress
        {
            Hotfix = 0,
            DB2 = 5,
            Stores = 85,
            MySQLSpells = 90,
            GtScaling = 95,
            Completed = 100
        }

        public static async Task Load(Action<int> progressCallback)
        {
            var progressHandler = new ProgressHandler(progressCallback);

            HotfixReader hotfixReader = null;
            try
            {
                hotfixReader = new HotfixReader(Settings.Default.HotfixCachePath);
            }
            catch (Exception)
            {
                Console.WriteLine(
                    $"Hotfix cache {Settings.Default.HotfixCachePath} cannot be loaded, ignoring!");
            }

            progressHandler.SetProgress((int)Progress.DB2);

            var dbcProperties = typeof(DBC).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var dbcPropertiesFiltered = dbcProperties.Where(dbc => dbc.PropertyType.IsGenericType && dbc.PropertyType.GetGenericTypeDefinition() == typeof(Storage<>));
            progressHandler.StartStepsProgress(dbcPropertiesFiltered.Count(), (int)Progress.Stores);

            Parallel.ForEach(dbcProperties, dbc =>
               {
                   var name = dbc.Name;

                   try
                   {
                       var db2Reader = new DBReader($@"{Settings.Default.DbcPath}\{Settings.Default.Locale}\{name}.db2");

                       dynamic storage = Activator.CreateInstance(dbc.PropertyType, db2Reader);

                       if (hotfixReader != null)
                           hotfixReader.ApplyHotfixes(storage, db2Reader);

                       dbc.SetValue(dbc.GetValue(null), storage);
                   }
                   catch (DirectoryNotFoundException)
                   {
                   }
                   catch (TargetInvocationException tie)
                   {
                       if (tie.InnerException is ArgumentException)
                           throw new ArgumentException($"Failed to load {name}.db2: {tie.InnerException.Message}");
                       throw;
                   }
                   finally
                   {
                       progressHandler.IncrementStepsProgress();
                   }
               });

            progressHandler.SetProgress((int)Progress.Stores);

            foreach (var spell in SpellName)
                SpellInfoStore[(int)spell.Value.ID] = new SpellInfo(spell.Value, Spell.GetValue((int)spell.Value.ID));

            progressHandler.StartStepsProgress(ProgressStoresSteps, (int)Progress.MySQLSpells);
            await Task.WhenAll(Task.Run(() =>
            {
                foreach (var spellMisc in SpellMisc.Values.Where(misc => SpellInfoStore.ContainsKey(misc.SpellID)))
                {
                    if (spellMisc.DifficultyID != 0)
                        continue;

                    var spell = SpellInfoStore[spellMisc.SpellID];

                    spell.Misc = spellMisc;

                    if (SpellDuration.ContainsKey(spellMisc.DurationIndex))
                        spell.DurationEntry = SpellDuration[spellMisc.DurationIndex];

                    if (SpellRange.ContainsKey(spellMisc.RangeIndex))
                        spell.Range = SpellRange[spellMisc.RangeIndex];
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var effect in SpellEffect.Values)
                {
                    if (!SpellInfoStore.ContainsKey(effect.SpellID))
                    {
                        Console.WriteLine(
                            $"Spell effect {effect.ID} is referencing unknown spell {effect.SpellID}, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.SpellID].Effects.Add(effect);
                    SpellInfoStore[effect.SpellID].SpellEffectInfoStore[effect.EffectIndex] = new SpellEffectInfo(effect); // Helper

                    var triggerId = effect.EffectTriggerSpell;
                    if (triggerId != 0)
                    {
                        if (SpellTriggerStore.ContainsKey(triggerId))
                            SpellTriggerStore[triggerId].Add(effect.SpellID);
                        else
                            SpellTriggerStore.Add(triggerId, new SortedSet<int> { effect.SpellID });
                    }
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellTargetRestrictions in SpellTargetRestrictions.Values)
                {
                    if (spellTargetRestrictions.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(spellTargetRestrictions.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellTargetRestrictions: Unknown spell {spellTargetRestrictions.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellTargetRestrictions.SpellID].TargetRestrictions = spellTargetRestrictions;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellXSpellVisual in SpellXSpellVisual.Values.Where(effect => effect.CasterPlayerConditionID == 0))
                {
                    if (spellXSpellVisual.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(spellXSpellVisual.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellXSpellVisual: Unknown spell {spellXSpellVisual.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellXSpellVisual.SpellID].SpellXSpellVisual = spellXSpellVisual;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellScaling in SpellScaling.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellScaling.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellScaling: Unknown spell {spellScaling.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellScaling.SpellID].Scaling = spellScaling;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellAuraOptions in SpellAuraOptions.Values)
                {
                    if (spellAuraOptions.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(spellAuraOptions.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellAuraOptions: Unknown spell {spellAuraOptions.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellAuraOptions.SpellID].AuraOptions = spellAuraOptions;
                    if (spellAuraOptions.SpellProcsPerMinuteID != 0)
                        SpellInfoStore[spellAuraOptions.SpellID].ProcsPerMinute = SpellProcsPerMinute[spellAuraOptions.SpellProcsPerMinuteID];
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellAuraRestrictions in SpellAuraRestrictions.Values)
                {
                    if (spellAuraRestrictions.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(spellAuraRestrictions.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellAuraRestrictions: Unknown spell {spellAuraRestrictions.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellAuraRestrictions.SpellID].AuraRestrictions = spellAuraRestrictions;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellCategories in SpellCategories.Values)
                {
                    if (spellCategories.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(spellCategories.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCategories: Unknown spell {spellCategories.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellCategories.SpellID].Categories = spellCategories;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellCastingRequirements in SpellCastingRequirements.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellCastingRequirements.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCastingRequirements: Unknown spell {spellCastingRequirements.SpellID} referenced, ignoring!");
                        return;
                    }

                    SpellInfoStore[spellCastingRequirements.SpellID].CastingRequirements = spellCastingRequirements;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellClassOptions in SpellClassOptions.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellClassOptions.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellClassOptions: Unknown spell {spellClassOptions.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellClassOptions.SpellID].ClassOptions = spellClassOptions;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellCooldowns in SpellCooldowns.Values)
                {
                    if (spellCooldowns.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(spellCooldowns.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCooldowns: Unknown spell {spellCooldowns.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellCooldowns.SpellID].Cooldowns = spellCooldowns;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var effect in SpellInterrupts)
                {
                    if (effect.Value.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellInterrupts: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Interrupts = effect.Value;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellEquippedItems in SpellEquippedItems.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellEquippedItems.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellEquippedItems: Unknown spell {spellEquippedItems.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellEquippedItems.SpellID].EquippedItems = spellEquippedItems;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellLabel in SpellLabel.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellLabel.SpellID))
                    {
                        Console.WriteLine($"SpellLabel: Unknown spell {spellLabel.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellLabel.SpellID].Labels.Add(spellLabel.LabelID);
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellLevels in SpellLevels.Values)
                {
                    if (spellLevels.DifficultyID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(spellLevels.SpellID))
                    {
                        Console.WriteLine($"SpellLevels: Unknown spell {spellLevels.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellLevels.SpellID].Levels = spellLevels;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellPower in SpellPower.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellPower.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellPower: Unknown spell {spellPower.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellPower.SpellID].Powers.Add(spellPower);
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellReagents in SpellReagents.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellReagents.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellReagents: Unknown spell {spellReagents.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellReagents.SpellID].Reagents = spellReagents;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellReagentsCurrency in SpellReagentsCurrency.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellReagentsCurrency.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellReagentsCurrency: Unknown spell {spellReagentsCurrency.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellReagentsCurrency.SpellID].ReagentsCurrency.Add(spellReagentsCurrency);
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellShapeshift in SpellShapeshift.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellShapeshift.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellShapeshift: Unknown spell {spellShapeshift.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellShapeshift.SpellID].Shapeshift = spellShapeshift;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var spellTotems in SpellTotems.Values)
                {
                    if (!SpellInfoStore.ContainsKey(spellTotems.SpellID))
                    {
                        Console.WriteLine($"SpellTotems: Unknown spell {spellTotems.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[spellTotems.SpellID].Totems = spellTotems;
                }
                progressHandler.IncrementStepsProgress();
            }), Task.Run(() =>
            {
                foreach (var descriptionVariable in SpellXDescriptionVariables.Values)
                {
                    if (!SpellInfoStore.ContainsKey(descriptionVariable.SpellID))
                    {
                        Console.WriteLine($"SpellXDescriptionVariables: Unknown spell {descriptionVariable.SpellID} referenced, ignoring!");
                        continue;
                    }
                    SpellInfoStore[descriptionVariable.SpellID].DescriptionVariables = SpellDescriptionVariables.GetValue(descriptionVariable.SpellDescriptionVariablesID);
                }
                progressHandler.IncrementStepsProgress();
            }));

            progressHandler.SetProgress((int)Progress.MySQLSpells);

            MySqlConnection.LoadServersideSpells();

            progressHandler.SetProgress((int)Progress.GtScaling);

            GameTable<GtSpellScalingEntry>.Open($@"{Settings.Default.GtPath}\SpellScaling.txt");

            progressHandler.SetProgress((int)Progress.Completed);
        }

        public static uint SelectedLevel = MaxLevel;
        public static uint SelectedItemLevel = 475;
        public static MapDifficultyEntry SelectedMapDifficulty;
    }
}

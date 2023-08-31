﻿using FloraFixer;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SynFloraFixer
{
    public class Program
    {
        public static Lazy<Settings> Settings = null!;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(
                    nickname: "Settings",
                    path: "settings.json",
                    out Settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynFloraFix.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var floraFixScript = new VirtualMachineAdapter();
            floraFixScript.Scripts.Add(new ScriptEntry()
            {
                Name = string.IsNullOrWhiteSpace(Settings.Value.ScriptName) ? "florafix" : Settings.Value.ScriptName,
            });

            state.LoadOrder.PriorityOrder.OnlyEnabled().Tree().WinningOverrides().ForEach(tree =>
            {
                if (tree.VirtualMachineAdapter == null && tree.Ingredient != null && !tree.Ingredient.IsNull)
                {
                    var otree = state.PatchMod.Trees.GetOrAddAsOverride(tree);
                    Console.WriteLine($"Patching TREE {otree.EditorID}");
                    otree.VirtualMachineAdapter = floraFixScript;

                    SettingsProduction(otree.Production);
                }
            });
            state.LoadOrder.PriorityOrder.OnlyEnabled().Flora().WinningOverrides().ForEach(flora =>
            {
                if (flora.VirtualMachineAdapter == null && flora.Ingredient != null && !flora.Ingredient.IsNull)
                {
                    var otree = state.PatchMod.Florae.GetOrAddAsOverride(flora); ;
                    Console.WriteLine($"Patching FLOR {otree.EditorID}");
                    otree.VirtualMachineAdapter = floraFixScript;

                    var newText = SettingsActivateString(otree.ActivateTextOverride);
                    if (newText != null)
                    {
                        otree.ActivateTextOverride = newText;
                    }

                    SettingsProduction(otree.Production);
                }
            });
        }

        private static void SettingsProduction(SeasonalIngredientProduction? production)
        {
            bool isNull = false;
            if (production == null)
            {
                isNull = true;
                production = new SeasonalIngredientProduction();
            }

            if (isNull ||
            (production.Fall == 0
            && production.Spring == 0
            && production.Summer == 0
            && production.Winter == 0)
            )
            {
                production.Fall = 100;
                production.Spring = 100;
                production.Summer = 100;
                production.Winter = 100;
            }
        }

        private static TranslatedString? SettingsActivateString(ITranslatedStringGetter? translatedText)
        {
            if (translatedText == null || string.IsNullOrWhiteSpace(translatedText?.String))
            {
                return new TranslatedString(Language.English, new Dictionary<Language, string>
                        {
                            { Language.English, "Get:" },
                            { Language.Russian, "Взять:" },
                            { Language.Chinese, "获取：" },
                            { Language.Spanish, "Obtener:" },
                            { Language.French, "Obtenir:" },
                            { Language.German, "Erhalten:" },
                        });
            }

            return null;
        }
    }
}

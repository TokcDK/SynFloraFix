using System;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace SynFloraFixer
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynFloraFix.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            state.LoadOrder.PriorityOrder.OnlyEnabled().Tree().WinningOverrides().ForEach(tree =>
            {
                if (tree.VirtualMachineAdapter == null)
                {
                    var otree = state.PatchMod.Trees.GetOrAddAsOverride(tree);
                    Console.WriteLine($"Patching TREE {otree.EditorID}");
                    otree.VirtualMachineAdapter = new VirtualMachineAdapter();
                    otree.VirtualMachineAdapter.Scripts.Add(new ScriptEntry()
                    {
                        Name = "florafix",
                    });
                }
            });
            state.LoadOrder.PriorityOrder.OnlyEnabled().Flora().WinningOverrides().ForEach(flora =>
            {
                if (flora.VirtualMachineAdapter == null)
                {
                    var otree = state.PatchMod.Florae.GetOrAddAsOverride(flora); ;
                    Console.WriteLine($"Patching FLOR {otree.EditorID}");
                    otree.VirtualMachineAdapter = new VirtualMachineAdapter();
                    otree.VirtualMachineAdapter.Scripts.Add(new ScriptEntry()
                    {
                        Name = "florafix",
                    });
                }
            });
        }
    }
}

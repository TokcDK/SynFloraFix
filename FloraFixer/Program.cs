using System;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;

namespace SynFloraFixer
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance.AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch).Run(args, new RunPreferences()
            {
                ActionsForEmptyArgs = new RunDefaultPatcher
                {
                    IdentifyingModKey = "FloraFixer.esp",
                    TargetRelease = GameRelease.SkyrimSE
                }
            });
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var tree in state.LoadOrder.PriorityOrder.OnlyEnabled().Tree().WinningOverrides())
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
            }
            foreach (var flora in state.LoadOrder.PriorityOrder.OnlyEnabled().Flora().WinningOverrides())
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
            }
        }
    }
}

using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;

namespace enchrestrictionremover
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                new UserPreferences() {
                    ActionsForEmptyArgs = new RunDefaultPatcher
                    {
                        IdentifyingModKey = "FloraFixer.esp",
                        TargetRelease = GameRelease.SkyrimSE
                    }
                });
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach(var tree in state.LoadOrder.PriorityOrder.OnlyEnabled().Tree().WinningOverrides()) {
                var otree = state.PatchMod.Trees.GetOrAddAsOverride(tree);
                if(otree.VirtualMachineAdapter == null) {
                    otree.VirtualMachineAdapter = new VirtualMachineAdapter();
                }
                otree.VirtualMachineAdapter.Scripts.Add(new ScriptEntry() {
                    Name = "florafix",
                });
            }
            foreach(var flora in state.LoadOrder.PriorityOrder.OnlyEnabled().Flora().WinningOverrides()) {
                var otree = state.PatchMod.Florae.GetOrAddAsOverride(flora);;
                if(otree.VirtualMachineAdapter == null) {
                    otree.VirtualMachineAdapter = new VirtualMachineAdapter();
                }
                otree.VirtualMachineAdapter.Scripts.Add(new ScriptEntry() {
                    Name = "florafix",
                });
            }
        }
    }
}

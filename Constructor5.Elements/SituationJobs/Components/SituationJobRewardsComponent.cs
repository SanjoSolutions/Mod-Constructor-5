using Constructor5.Base.ElementSystem;
using Constructor5.Base.ExportSystem.Tuning;
using Constructor5.Base.ExportSystem.Tuning.Utilities;
using Constructor5.Core;
using System;

namespace Constructor5.Elements.SituationJobs.Components
{
    [XmlSerializerExtraType]
    public class SituationJobRewardsComponent : SituationJobComponent
    {
        public override string ComponentLabel => "Actions";

        public ReferenceList LootActionSets { get; set; } = new ReferenceList();

        protected internal override void OnExport(SituationJobExportContext context)
        {
            if (LootActionSets.Items.Count == 0)
            {
                return;
            }

            var tunableList1 = context.Tuning.Get<TunableList>("rewards");
            TuneLoot(tunableList1, "BRONZE", LootActionSets, (item) => item.RunOnBronze);
            TuneLoot(tunableList1, "SILVER", LootActionSets, (item) => item.RunOnSilver);
            TuneLoot(tunableList1, "GOLD", LootActionSets, (item) => item.RunOnGold);
            TuneLoot(tunableList1, "TIN", LootActionSets, (item) => item.RunOnNoMedal);
        }

        private void TuneLoot(TunableList tunableList1, string medalLevel, ReferenceList referenceList, Func<SituationJobRewardLootItem, bool> func)
        {
            if (referenceList.HasItems())
            {
                var tunableTuple1 = tunableList1.Get<TunableTuple>(null);
                tunableTuple1.Set<TunableEnum>("medal", medalLevel);
                var tunableTuple2 = tunableTuple1.Get<TunableTuple>("benefit");
                var tunableList2 = tunableTuple2.Get<TunableList>("loot");

                foreach(var item in referenceList.GetOfType<SituationJobRewardLootItem>())
                {
                    foreach (var loot in ElementTuning.GetInstanceKeys(item.Reference))
                    {
                        if (func.Invoke(item))
                        {
                            tunableList2.Set<TunableBasic>(null, loot);
                        }
                    }
                }
            }
        }
    }
}

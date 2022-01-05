﻿using Constructor5.Base.ElementSystem;
using Constructor5.Base.ExportSystem.Tuning;
using Constructor5.Base.ExportSystem.Tuning.Utilities;
using Constructor5.Base.SelectableObjects;
using Constructor5.Elements.TestConditions;
using Constructor5.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Constructor5.Elements.SituationGoals.Templates
{
    [XmlSerializerExtraType]
    [SelectableObjectType("SituationGoalTemplates", "Interaction Goal")]
    public class SituationGoalInteractionTemplate : SituationGoalTemplate
    {
        public bool IncludeCancelledInteractions { get; set; }
        public bool InSituationOnly { get; set; }
        public ReferenceList Interactions { get; set; } = new ReferenceList();
        public ObservableCollection<string> InteractionTags { get; } = new ObservableCollection<string>();
        public override string Label => "Interaction Goal";
        public int MinimumRunningTime { get; set; }
        public SituationGoalInteractionTarget SpecificTarget { get; set; }
        public bool SuccessfulOnly { get; set; }
        public ObservableCollection<TestConditionListItem> TargetConditions { get; set; } = new ObservableCollection<TestConditionListItem>();
        public bool WaitUntilCompletion { get; set; }

        protected override void OnExport(SituationGoalExportContext context)
        {
            var tuningHeader = (TuningHeader)context.Tuning;

            var isTargeted = InSituationOnly || SpecificTarget != SituationGoalInteractionTarget.Any || TargetConditions.Count > 0;

            if (isTargeted)
            {
                tuningHeader.Class = "SituationGoalRanInteractionOnTargetedSim";
                tuningHeader.Module = "situations.situation_goal_targeted_sim";
            }
            else
            {
                tuningHeader.Class = "SituationGoalObjectInteraction";
                tuningHeader.Module = "situations.situation_goal_object_interaction";
            }

            var tunableTuple1 = context.Tuning.Get<TunableTuple>("_goal_test");

            foreach (var interaction in ElementTuning.GetInstanceKeys(Interactions))
            {
                var tunableList1 = tunableTuple1.Get<TunableList>("affordances");
                tunableList1.Set<TunableBasic>(null, interaction);
            }

            foreach (var tag in InteractionTags)
            {
                var tunableList2 = tunableTuple1.Get<TunableList>("tags");
                tunableList2.Set<TunableEnum>(null, tag);
            }

            if (IncludeCancelledInteractions)
            {
                tunableTuple1.Set<TunableBasic>("consider_all_cancelled_as_failure", "False");
            }

            if (SuccessfulOnly)
            {
                var tunableVariant1 = tunableTuple1.Set<TunableVariant>("interaction_outcome", "enabled");
                tunableVariant1.Set<TunableEnum>("enabled", "SUCCESS");
            }

            if (MinimumRunningTime > 0)
            {
                tunableTuple1.SetVariant<TunableBasic>("running_time", "enabled", MinimumRunningTime.ToString());
            }

            if (!WaitUntilCompletion && !SuccessfulOnly)
            {
                if (MinimumRunningTime > 0)
                {
                    tunableTuple1.Set<TunableEnum>("test_event", "InteractionUpdate");
                }
                else
                {
                    tunableTuple1.Set<TunableEnum>("test_event", "InteractionStart");
                }
            }

            var conditions = new List<TestCondition>();
            foreach (var condition in TargetConditions)
            {
                conditions.Add(condition.Condition);
            }
            TestConditionTuning.TuneTestConditions(context.Tuning, conditions, "_target_tests");

            if (isTargeted && !InSituationOnly)
            {
                context.Tuning.Set<TunableBasic>("_select_sims_outside_of_situation", "True");
            }

            switch (SpecificTarget)
            {
                case SituationGoalInteractionTarget.Random:
                    context.Tuning.Set<TunableEnum>("_target_option", "GoalSystemChoice");
                    break;
                case SituationGoalInteractionTarget.Inherited:
                    context.Tuning.Set<TunableEnum>("_target_option", "Inherited");
                    break;
                case SituationGoalInteractionTarget.RandomExcludingInherited:
                    context.Tuning.Set<TunableEnum>("_target_option", "GoalSystemChoiceExcludingInherited");
                    break;
            }
        }
    }
}
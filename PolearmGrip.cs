using System;
using System.Collections.Generic;
using XRL.Messages;
using XRL.Rules;
using XRL.UI;
using XRL.Language;
using XRL.World.Effects;
using XRL.World.AI.GoalHandlers;
using XRL.World.Parts;


namespace XRL.World.Parts.Skill
{
	[Serializable]
	public class acegiak_Polearm_Grip : BaseSkill
	{
		public Guid ActivatedAbilityID = Guid.Empty;
        public ActivatedAbilityEntry Ability = null;
        
		public acegiak_Polearm_Grip()
		{
			DisplayName = "Polearm_Grip";
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "DealDamage");
			Object.RegisterPartEvent(this, "CommandAcegiakPolearmGrip");
			Object.RegisterPartEvent(this, "AIGetOffensiveMutationList");
			Object.RegisterPartEvent(this, "BeginEquip");
			Object.RegisterPartEvent(this, "EquipperUnequipped");
			base.Register(Object);
		}


        public override bool AddSkill(GameObject GO)
        {
            ActivatedAbilities pAA = GO.GetPart("ActivatedAbilities") as ActivatedAbilities;

            if (pAA != null)
            {
                ActivatedAbilityID = pAA.AddAbility("Double Grip", "CommandAcegiakPolearmGrip", "Skill", "You wield your polearm with two hands for additional damage.", "G",null,true,false);
                Ability = pAA.AbilityByGuid[ActivatedAbilityID];
            }

            return true;
        }

        public override bool RemoveSkill(GameObject GO)
        {
            if (ActivatedAbilityID != Guid.Empty)
            {
                ActivatedAbilities pAA = GO.GetPart("ActivatedAbilities") as ActivatedAbilities;
                pAA.RemoveAbility(ActivatedAbilityID);
            }

            return true;
        }
		
		public GameObject GetPrimaryPolearm()
        {
            GameObject result = null;
			int num = 0;
			Body part = ParentObject.GetPart<Body>();
			List<BodyPart> equippedParts = part.GetEquippedParts();
			foreach (BodyPart item in equippedParts)
			{
				XRL.World.Parts.acegiak_Reach part2 = item.Equipped.GetPart<XRL.World.Parts.acegiak_Reach>();
				if (part2 != null)
				{
					result = item.Equipped;
				}
			}
			return result;        }

        public bool IsPrimaryPolearmEquipped()
        {
            return GetPrimaryPolearm() != null;
        }

		public void reEquip(GameObject who,GameObject what){
				GameObject equipped = what.pPhysics.Equipped;
				BodyPart bodyPart = equipped.FindEquippedObject(what);
				if (bodyPart != null)
				{
					equipped.FireEvent(Event.New("CommandForceUnequipObject", "BodyPart", bodyPart));
					equipped.FireEvent(Event.New("CommandForceEquipObject", "Object", what, "BodyPart", bodyPart));
				}
		}
		


		public override bool FireEvent(Event E)
		{
			if (E.ID == "AIGetOffensiveMutationList")
			{
                int Distance = E.GetIntParameter("Distance");
                GameObject Target = E.GetGameObjectParameter("Target");
                if (Target == null) return true;
                if (!IsPrimaryPolearmEquipped()) return true;
                if (ParentObject.pPhysics != null && ParentObject.pPhysics.IsFrozen()) return true;
                List<XRL.World.AI.GoalHandlers.AICommandList> CommandList = (List<XRL.World.AI.GoalHandlers.AICommandList>)E.GetParameter("List");
                if (Ability != null && Ability.Cooldown <= 0 && Distance <= 1) CommandList.Add(new XRL.World.AI.GoalHandlers.AICommandList("CommandAcegiakPolearmGrip", 1));
                return true;
			

			}else if (E.ID == "CommandAcegiakPolearmGrip")
			{
				if (Ability == null)
				{
					return true;
				}
				Ability.ToggleState = !Ability.ToggleState;
                if(IsPrimaryPolearmEquipped()){
                    if(Ability.ToggleState){
                        GetPrimaryPolearm().ApplyEffect(new acegiak_Gripped());
						reEquip(ParentObject,GetPrimaryPolearm());
                    }else{
                        GetPrimaryPolearm().RemoveEffect(typeof(acegiak_Gripped));
						reEquip(ParentObject,GetPrimaryPolearm());
                    }
                }
			}

			if (E.ID == "BeginEquip")
			{
				if (Ability == null)
				{
					return true;
				}
				GameObject GO = E.GetParameter<GameObject>("Object");
                if(GO.GetPart<acegiak_Reach>() != null){
                    if(Ability.ToggleState){
                        GO.ApplyEffect(new acegiak_Gripped());
                    }else{
                        GO.RemoveEffect(typeof(acegiak_Gripped));
                    }
                }
				return true;
			}
			if (E.ID == "EquipperUnequipped")
			{
				GameObject GO = E.GetParameter<GameObject>("Object");
                if(GO.GetPart<acegiak_Reach>() != null){
                    GO.RemoveEffect(typeof(acegiak_Gripped));
                }
				return true;
			}
			return base.FireEvent(E);
		}


	}

}

using System;
using System.Collections.Generic;
using XRL.Messages;
using XRL.Rules;
using XRL.UI;
using XRL.Language;
using XRL.World.Effects;
using XRL.World.AI.GoalHandlers;


namespace XRL.World.Parts.Skill
{
	[Serializable]
	public class acegiak_Polearm_Repel : BaseSkill
	{
		public Guid ActivatedAbilityID = Guid.Empty;
        public ActivatedAbilityEntry Ability = null;
        
		public acegiak_Polearm_Repel()
		{
			DisplayName = "Polearm_Repel";
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "AttackerHit");
			Object.RegisterPartEvent(this, "CommandAcegiakPolearmRepel");
			Object.RegisterPartEvent(this, "AIGetOffensiveMutationList");
			base.Register(Object);
		}


        public override bool AddSkill(GameObject GO)
        {
            ActivatedAbilities pAA = GO.GetPart("ActivatedAbilities") as ActivatedAbilities;

            if (pAA != null)
            {
                ActivatedAbilityID = pAA.AddAbility("Repel Attackers", "CommandAcegiakPolearmRepel", "Skill","When attacking with a polearm you push enemies away from you.", ">", null, true,true);
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
                if (Ability != null && Ability.Cooldown <= 0 && Distance <= 1) CommandList.Add(new XRL.World.AI.GoalHandlers.AICommandList("CommandAcegiakPolearmRepel", 1));
                return true;
			}
			if (E.ID == "AttackerHit"){
                //IPart.AddPlayerMessage("Damage Dealt");
				if(Ability != null && Ability.ToggleState){
                    //IPart.AddPlayerMessage("Repel Active");
					GameObject weapon = E.GetParameter("Weapon") as GameObject;
					if (weapon != null && weapon.GetPart<XRL.World.Parts.acegiak_Reach>() != null)
					{
                        //IPart.AddPlayerMessage("Move them!");
						GameObject defender = E.GetParameter("Defender") as GameObject;
						GameObject attacker = E.GetParameter("Attacker") as GameObject;
						Slam(defender,attacker.pPhysics.CurrentCell.GetDirectionFromCell(defender.pPhysics.CurrentCell));
					}
				}

			}
			if (E.ID == "CommandAcegiakPolearmRepel")
			{
				if (Ability == null)
				{
					return true;
				}
				Ability.ToggleState = !Ability.ToggleState;
			}
			return base.FireEvent(E);
		}

        public bool Slam(GameObject target, string sDirection)
        {
            //IPart.AddPlayerMessage("Slammin");

            if (target.IsInvalid()){
				 return false;
			}


            Cell C = target.pPhysics.CurrentCell.GetLocalCellFromDirection(sDirection);

            if (C == null)
            {
                return false;
            }

            if(!target.MakeSave("Strength",20,ParentObject,null,"Polearm Repel")){

                if (C.IsEmpty() && target.pPhysics.Weight < 2000)
                {

                    
                    target.pPhysics.Push(sDirection, 1000, 4);
                    target.DustPuff();
                    return true;


                    // if (target.FireEvent(Event.New("CommandMove", "Direction", sDirection, "Forced", 1)))
                    // {

                    //     if( target.CurrentCell != null )
                    //     {
                    //         foreach( var cell in target.CurrentCell.GetAdjacentCells() )
                    //         {
                    //             cell.FireEvent( Event.New("ObjectEnteredAdjacentCell", "Object", target ));
                    //         }
                    //     }
                    //     return true;
                    // }
                    // else
                    // {
                    //     return false;
                    // }
                }
                else
                {			
                    return false;
                }
            }
            return false;
        }
	}
}

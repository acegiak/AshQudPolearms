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
	public class acegiak_Polearm_Lunge : BaseSkill
	{
		public ActivatedAbilityEntry pActivatedAbility;

		public Guid ActivatedAbilityID = Guid.Empty;
        
		public acegiak_Polearm_Lunge()
		{
			DisplayName = "Polearm_Lunge";
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "CommandAcegiakPolearmLunge");
			Object.RegisterPartEvent(this, "AIGetOffensiveMutationList");
			Object.RegisterPartEvent(this, "AttackerGetWeaponPenModifier");
			base.Register(Object);
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
			return result;
        }

        public bool IsPrimaryPolearmEquipped()
        {
            return GetPrimaryPolearm() != null;
        }

		private bool ValidLungeTarget(GameObject obj)
		{
			return obj?.HasPart("Combat") ?? false;
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "AIGetOffensiveMutationList")
			{
				int intParameter = E.GetIntParameter("Distance");
				GameObject gameObjectParameter = E.GetGameObjectParameter("Target");
				List<AICommandList> list = (List<AICommandList>)E.GetParameter("List");
				if (pActivatedAbility != null && !ParentObject.AreHostilesAdjacent() && pActivatedAbility.Cooldown <= 0 && intParameter > 3 && !ParentObject.AreHostilesAdjacent() && intParameter < 6 + ParentObject.GetIntProperty("LungeRangeModifier") && ParentObject.HasLOSTo(gameObjectParameter))
				{
					List<Cell> list2 = PickLine(2, AllowVis.OnlyVisible);
					if (list2 == null)
					{
						return true;
					}
					if (list2.Count != 2)
					{
						return true;
					}
					for (int i = 0; i < list2.Count; i++)
					{
						foreach (GameObject item in list2[i].LoopObjectsWithPart("Combat"))
						{
							if (ParentObject.HasPart("Brain") && ParentObject.GetPart<Brain>().GetFeeling(item) >= 0)
							{
								return true;
							}
						}
					}
					list.Add(new AICommandList("CommandAcegiakPolearmLunge", 1));
				}
				return true;
			}
			if (E.ID == "CommandAcegiakPolearmLunge")
			{
                if(!IsPrimaryPolearmEquipped()){
					if (ParentObject.IsPlayer())
					{
						Popup.Show("You must have a polearm equipped to Polearm Lunge.");
					}
					return true;
                }
				if (ParentObject.OnWorldMap())
				{
					if (ParentObject.IsPlayer())
					{
						Popup.Show("You cannot Lunge on the world map.");
					}
					return true;
				}
				if (ParentObject.pPhysics != null && ParentObject.pPhysics.IsFrozen())
				{
					Popup.Show("You are frozen solid!");
					return true;
				}
				List<Cell> list3 = PickLine(2, AllowVis.OnlyVisible, ValidLungeTarget);
				if (list3 == null)
				{
					return true;
				}
				int num = 3;
				int num2 = 2;
				if (list3.Count != num)
				{
					if (IsPlayer())
					{
						Popup.Show("You must Lunge 2 spaces.");
					}
					return false;
				}
				if (!list3[list3.Count - 1].HasObjectWithPart("Combat"))
				{
					if (IsPlayer())
					{
						Popup.Show("You must Lunge at a target!");
					}
					return false;
				}
				Physics pPhysics = ParentObject.pPhysics;
				string text = ParentObject.pRender.ColorString + ParentObject.pRender.RenderString;
				if (ParentObject.IsPlayer())
				{
					list3.RemoveAt(0);
				}
				int num3 = 10;
				foreach (Cell item2 in list3)
				{
					List<GameObject> objectsWithPart = item2.GetObjectsWithPart("Combat");
					if (objectsWithPart.Count > 0)
					{
						if (objectsWithPart[0].IsPlayer())
						{
							IPart.AddPlayerMessage("&R" + ParentObject.The + ParentObject.DisplayName + " &R" + ParentObject.GetVerb("lunge", PrependSpace: false) + " you!");
						}
						if (ParentObject.DistanceTo(item2) == 2)
						{
							ParentObject.FireEvent(Event.New("CommandAttackCell", "Cell", item2, "Properties", "Lunging"));
						}
						pActivatedAbility.Cooldown = 0;
						ParentObject.FireEvent(Event.New("LungedTarget", "Defender", objectsWithPart[0]));
						objectsWithPart[0].FireEvent(Event.New("WasLunged", "Attacker", ParentObject));
						return true;
					}
				}
			}
			return base.FireEvent(E);
		}

		public override bool AddSkill(GameObject GO)
		{
			ActivatedAbilities activatedAbilities = GO.GetPart("ActivatedAbilities") as ActivatedAbilities;
			if (activatedAbilities != null)
			{
				ActivatedAbilityID = activatedAbilities.AddAbility("Polearm Lunge [&Wattack&y]", "CommandAcegiakPolearmLunge", "Skill","You strike out with a polearm to attack a foe two spaces away.", string.Empty + '\u0010');
				pActivatedAbility = activatedAbilities.AbilityByGuid[ActivatedAbilityID];
			}
			return true;
		}

		public override bool RemoveSkill(GameObject GO)
		{
			if (ActivatedAbilityID != Guid.Empty)
			{
				ActivatedAbilities activatedAbilities = GO.GetPart("ActivatedAbilities") as ActivatedAbilities;
				activatedAbilities.RemoveAbility(ActivatedAbilityID);
				pActivatedAbility = null;
			}
			return true;
		}
	}
}

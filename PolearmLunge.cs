using System;
using System.Collections.Generic;
using XRL.Messages;
using XRL.Rules;
using XRL.UI;
using XRL.Language;
using XRL.World.Effects;
using XRL.World.AI.GoalHandlers;
using XRL.World.Anatomy;
using ConsoleLib.Console;

namespace XRL.World.Parts.Skill
{
	[Serializable]
	public class acegiak_Polearm_Lunge : BaseSkill
	{
		public ActivatedAbilityEntry Ability;

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
				if (Ability != null && !ParentObject.AreHostilesAdjacent() && Ability.Cooldown <= 0 && intParameter > 3 && !ParentObject.AreHostilesAdjacent() && intParameter < 6 + ParentObject.GetIntProperty("LungeRangeModifier") && ParentObject.HasLOSTo(gameObjectParameter))
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
					Popup.Show("Not a valid lunge target.");
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
				if(list3.Count <= 0){
					Popup.Show("Invalid target cell!");
					return false;
				}
				int num3 = 10;
				foreach (Cell targetcell in list3)
				{
					if (ParentObject.DistanceTo(targetcell) != 2){
						continue;
					}
					GameObject combattarget = targetcell.GetCombatTarget(ParentObject, IgnoreFlight: false, IgnoreAttackable: false, IgnorePhase: false, 5);
					if (combattarget != null)
					{
						GameObject primaryArm = ParentObject.GetPrimaryWeapon();
						
						if (combattarget.IsPlayer())
						{
							IPart.AddPlayerMessage("&R" + ParentObject.The + ParentObject.DisplayName + " &R" + ParentObject.GetVerb("lunge", PrependSpace: false) + " you!");
						}
						// Combat.MeleeAttackWithWeapon(ParentObject, combatTarget2, primaryBlade, ParentObject.Body.FindDefaultOrEquippedItem(primaryBlade), "Lunging", 0, 2, 2, 0, 0, Primary: true);

						Combat.MeleeAttackWithWeapon(ParentObject, combattarget, primaryArm, ParentObject.Body.FindDefaultOrEquippedItem(primaryArm), "Lunging", 0, 2, 2, 0, 0, Primary: true);

						Ability.Cooldown = 0;
						ParentObject.FireEvent(Event.New("LungedTarget", "Defender", combattarget));
						combattarget.FireEvent(Event.New("WasLunged", "Attacker", ParentObject));


						ParentObject?.PlayWorldSound("Sounds/Abilities/sfx_ability_longBlade_lunge");

						ParentObject.UseEnergy(1000, "Polearm Lunge");
						return true;
					}else{
						Popup.Show("No target found in cell!");
						return false;
					}
				}

				Popup.Show("No valid lunge space found.");
				return false;
			}
			return base.FireEvent(E);
		}

		public override bool AddSkill(GameObject GO)
		{

			ActivatedAbilityID = AddMyActivatedAbility("Polearm Lunge", "CommandAcegiakPolearmLunge", "Skill","You strike out with a polearm to attack a foe two spaces away.", "G", null, Toggleable: false,  IsAttack: true, IsRealityDistortionBased: false, IsWorldMapUsable: false, UITileDefault : Renderable.UITile("abilities/polearmlunge.png", foregroundColorCode : 'Y', detailColorCode : 'W', noTileAlt : "G", noTileColor : '\0'));
			Ability = ParentObject.ActivatedAbilities?.GetAbility(ActivatedAbilityID);
            
			return true;
		}

		public override bool RemoveSkill(GameObject GO)
		{
            if (ActivatedAbilityID != Guid.Empty)
            {
                RemoveMyActivatedAbility(ref ActivatedAbilityID);
            }

			return true;
		}
	}
}

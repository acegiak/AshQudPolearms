using System;

namespace XRL.World.Parts.Skill
{
	[Serializable]
	class acegiak_Polearm_Expertise : BaseSkill
	{
		public int HitBonus = 2;

		public acegiak_Polearm_Expertise()
		{
			DisplayName = "Polearm_Expertise";
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "AttackerRollMeleeToHit");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "AttackerRollMeleeToHit")
			{
				string stringParameter = E.GetStringParameter("Skill");
				if (stringParameter == "Polearm" && HitBonus != 0)
				{
					E.SetParameter("Result", E.GetIntParameter("Result", 0) + HitBonus);
				}
			}
			return base.FireEvent(E);
		}

		public override bool AddSkill(GameObject GO)
		{
			return true;
		}

		public override bool RemoveSkill(GameObject GO)
		{
			return true;
		}
	}
}
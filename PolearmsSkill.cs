using System;

namespace XRL.World.Parts.Skill
{
	[Serializable]
	public class acegiak_Polearms : BaseSkill
	{
		public acegiak_Polearms()
		{
			DisplayName = "Polearm";
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
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

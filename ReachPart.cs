using System;
using System.Collections.Generic;
using System.Text;

using XRL.World.Anatomy;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Reach : IPart
	{
		public int Length;
			
		public acegiak_Reach()
		{
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetShortDescription");
			Object.RegisterPartEvent(this, "AttackerGetWeaponPenModifier");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{

			if (E.ID == "AttackerGetWeaponPenModifier")
			{
				if (E.HasParameter("Properties") && E.GetStringParameter("Properties") != null && E.GetStringParameter("Properties").Contains("Charging") && E.HasParameter("Hand") && E.GetStringParameter("Hand") == "Primary")
				{
					E.SetParameter("PenBonus", E.GetIntParameter("PenBonus") + 1);
					E.SetParameter("CapBonus", E.GetIntParameter("CapBonus") + 1);
				}
			}

			if (E.ID == "GetShortDescription")
			{
	      		if(ParentObject.GetPart<acegiak_ModExtended>() == null){
					E.SetParameter("Postfix", E.GetParameter("Postfix") + "\n&CPolearm: Can be used to fight with additional reach.");
				}
			}
			return base.FireEvent(E);
		}
  }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Reach : IPart
	{
		public int Length;
			
		public acegiak_Reach()
		{
			base.Name = "acegiak_Reach";
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetShortDescription");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
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
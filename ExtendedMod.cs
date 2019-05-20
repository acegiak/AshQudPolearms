using System;
using System.Text;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_ModExtended : IModification
	{
		public acegiak_ModExtended()
		{
		}

		public acegiak_ModExtended(int Tier)
			: base(Tier)
		{
		}

		public override void Configure()
		{
			WorksOnSelf = true;
			NameForStatus = "ExtendedEnhancement";
		}

		public override bool ModificationApplicable(GameObject Object)
		{
			MeleeWeapon part = Object.GetPart<MeleeWeapon>();
			if (part == null)
			{
				return false;
			}

			return true;
		}

		public override void ApplyModification(GameObject Object)
		{
			Object.AddPart<XRL.World.Parts.acegiak_Reach>();
            Object.GetPart<XRL.World.Parts.Render>().Tile = "items/polearm.png";
			IncreaseDifficultyAndComplexityIfComplex(1, 1);
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetDisplayName");
			Object.RegisterPartEvent(this, "GetShortDisplayName");
			Object.RegisterPartEvent(this, "GetShortDescription");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "GetShortDescription")
			{
				E.SetParameter("Postfix", E.GetParameter("Postfix") + "\n&CExtended: Can be used as a polearm.");
			}
			if ((E.ID == "GetDisplayName" || E.ID == "GetShortDisplayName") && (!ParentObject.Understood() || !ParentObject.HasProperName))
			{
				E.GetParameter<StringBuilder>("Prefix").Append("extended ");
			}
			return base.FireEvent(E);
		}
	}
}

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
    }
}
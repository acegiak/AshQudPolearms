using System;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_Gripped : Effect
	{
        public bool usedTwoSlots;
        public string originalDamage;

		public acegiak_Gripped()
		{
			base.DisplayName = "&cDoubleGripped";
            this.Duration = Int32.MaxValue;
		}

	

		public override string GetDetails()
		{
			return "Dealing additional damage.";
		}

		public override bool CanApplyToStack()
		{
			return true;
		}

		public override bool Apply(GameObject Object)
		{
            if(Object.HasEffect(typeof(acegiak_Gripped))){
                return false;
            }
            usedTwoSlots = Object.pPhysics.bUsesTwoSlots;
            if(usedTwoSlots){
                return false;
            }
            if(!usedTwoSlots){
                Object.pPhysics.bUsesTwoSlots = true;
            }

            if(Object.GetPart<MeleeWeapon>() != null){
                originalDamage = Object.GetPart<MeleeWeapon>().BaseDamage;
                Object.GetPart<MeleeWeapon>().BaseDamage = diesizemod(originalDamage,2);
            }


			return true;
		}

        public override void Remove(GameObject Object){
            if(!usedTwoSlots){
                Object.pPhysics.bUsesTwoSlots = false;
            }

            if(Object.GetPart<MeleeWeapon>() != null){
                //Object.GetPart<MeleeWeapon>().BaseDamage = diesizemod(originalDamage,-1);
                Object.GetPart<MeleeWeapon>().BaseDamage = this.originalDamage;
            }
        }

		public override void Register(GameObject Object)
		{
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			base.Unregister(Object);
		}

		public override bool FireEvent(Event E)
		{
			return true;
		}


        public string diesizemod(string workingdamage, int mod){
                int diecount = 0;
                int bonus = 0;
                if(workingdamage.Contains("d")){
                    string[] ds = workingdamage.Split('d');
                    diecount = Int32.Parse(ds[0]);
                    workingdamage = ds[1];
                }
                if(workingdamage.Contains("+")){
                    string[] ds = workingdamage.Split('+');
                    bonus = Int32.Parse(ds[1]);
                    workingdamage = ds[0];
                }
                if(workingdamage.Contains("-")){
                    string[] ds = workingdamage.Split('-');
                    bonus = Int32.Parse(ds[1])*-1;
                    workingdamage = ds[0];
                }
                int diesize = Int32.Parse(workingdamage);
                diesize = diesize+mod;
                workingdamage = diesize.ToString();
                if(bonus > 0){
                    workingdamage +="+"+bonus.ToString();
                }
                if(bonus<0){
                    workingdamage +="-"+(bonus*-1).ToString();
                }
                if(diecount != 0){
                    workingdamage = diecount.ToString()+"d"+workingdamage;
                }
                return workingdamage;
        }
	}
}

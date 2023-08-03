using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Stigma_Execution : Stigma
{
    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        if (receiver.HP.GetCurrentHP() <= 10)
        {
            base.Use(caster, receiver);
            receiver.ChangeHP(-receiver.HP.GetCurrentHP());
        }
    }
}
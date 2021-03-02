using System.Collections.Generic;
using System.Linq;

namespace xggameplan.Model
{
    public class CampaignPriorityRoundsModel
    {
        public bool ContainsInclusionRound { get; set; }
        public List<PriorityRoundModel> Rounds { get; set; }

        public void PopulateRoundNumbers()
        {
            if (Rounds != null)
            {
                Rounds = Rounds.OrderBy(r => r.PriorityFrom).ToList();

                for (var i = 0; i < Rounds.Count; i++)
                {
                    Rounds[i].Number = i + 1;
                }
            }           
        }
    }
}

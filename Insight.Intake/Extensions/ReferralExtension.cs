using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Extensions
{
    public static class ReferralExtension
    {
        public static Incident ToIncident(this ipg_referral referral)
        {
            var incident = new Incident();
            incident.ipg_ActualDOS = null;
            incident.ipg_SurgeryDate = referral.ipg_SurgeryDate;
            incident.ipg_FacilityId = referral.ipg_FacilityId;
            incident.ipg_CarrierId = referral.ipg_CarrierId;
            incident.ipg_CPTCodeId1 = referral.ipg_CPTCodeId1;
            incident.ipg_CPTCodeId2 = referral.ipg_CPTCodeId2;
            incident.ipg_CPTCodeId3 = referral.ipg_CPTCodeId3;
            incident.ipg_CPTCodeId4 = referral.ipg_CPTCodeId4;
            incident.ipg_CPTCodeId5 = referral.ipg_CPTCodeId5;
            incident.ipg_CPTCodeId6 = referral.ipg_CPTCodeId6;
            return incident;
        }

        public static IList<Guid> GetNotesToCopyToCase(this ipg_referral referral)
        {
            string[] stringNoteIds = (referral.ipg_NotesToCopyToCase ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var noteIds = new List<Guid>();
            foreach (var stringNoteId in stringNoteIds)
            {
                if (Guid.TryParse(stringNoteId, out Guid noteId))
                {
                    noteIds.Add(noteId);
                }
            }

            return noteIds;
        }

        public static void SetNotesToCopyToCase(this ipg_referral referral, IList<Guid> notesToCopyToCase)
        {
            referral.ipg_NotesToCopyToCase = string.Join(",", notesToCopyToCase);
        }
    }
}

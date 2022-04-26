using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckDuplicateCase : PluginBase
    {
        public CheckDuplicateCase() : base(typeof(CheckDuplicateCase))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckDuplicateCase", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var targetRef = (EntityReference)localPluginContext.PluginExecutionContext.InputParameters["Target"];
            if (targetRef == null)
            {
                throw new InvalidPluginExecutionException("Target case is null");
            }
            string targetEntityFirstName = string.Empty;
            string targetEntityLastName = string.Empty;
            string caseorReferralNumber = string.Empty;

            DateTime targetEntityDOB = DateTime.MinValue;
            EntityCollection possibleDuplicateReferral = new EntityCollection();

            if (targetRef.LogicalName == Incident.EntityLogicalName)
            {
                var caseEntity = localPluginContext.OrganizationService.Retrieve(Incident.EntityLogicalName, targetRef.Id, new ColumnSet(
                                    Incident.Fields.ipg_PatientFirstName,
                                    Incident.Fields.ipg_PatientLastName,
                                    Incident.Fields.ipg_PatientDateofBirth,
                                    Incident.Fields.Title
                                    )).ToEntity<Incident>();
                targetEntityFirstName = caseEntity.ipg_PatientFirstName;
                targetEntityLastName = caseEntity.ipg_PatientLastName;
                targetEntityDOB = caseEntity.ipg_PatientDateofBirth.Value;

                caseorReferralNumber = caseEntity.Title;
            }
            else
            {

                if (targetRef.LogicalName == ipg_referral.EntityLogicalName)
                {
                    var referral = localPluginContext.OrganizationService.Retrieve(ipg_referral.EntityLogicalName, targetRef.Id, new ColumnSet(
                                        ipg_referral.Fields.ipg_PatientFirstName,
                                        ipg_referral.Fields.ipg_PatientLastName,
                                        ipg_referral.Fields.ipg_PatientDateofBirth,
                                        ipg_referral.Fields.ipg_referralcasenumber
                                        )).ToEntity<ipg_referral>();
                    targetEntityFirstName = referral.ipg_PatientFirstName;
                    targetEntityLastName = referral.ipg_PatientLastName;
                    targetEntityDOB = referral.ipg_PatientDateofBirth.Value;
                    caseorReferralNumber = referral.ipg_referralcasenumber;

                    possibleDuplicateReferral = GetPossibleDuplicateReferral(localPluginContext, targetEntityFirstName, targetEntityLastName, targetEntityDOB, targetRef);
                }
            }

            EntityCollection possibleDuplicateCaseID = GetPossibleDuplicateCase(localPluginContext, targetEntityFirstName, targetEntityLastName, targetEntityDOB);
                  
            if (possibleDuplicateCaseID.Entities.Any())
            {
                string duplicateCaseIds = string.Join(",", possibleDuplicateCaseID.Entities.Select(x => x.ToEntity<Incident>().Title).ToList<string>());

                if (possibleDuplicateReferral.Entities.Any() && possibleDuplicateCaseID.Entities.Any())
                {
                    string duplicateReferralIds = string.Join(",", possibleDuplicateReferral.Entities.
                                                                   Select(x => x.ToEntity<ipg_referral>().ipg_referralcasenumber).ToList<string>());
                    context.OutputParameters["CaseNote"] = $"Possible duplicate Referral. Referral {caseorReferralNumber} may be a duplicate of Referral {duplicateReferralIds} and Case {duplicateCaseIds}. " +
                    "Please review the Referral list to determine if these are true duplicates. Document your findings in the Task Note.";
                    context.OutputParameters["TaskDescripton"] = $"Referral {caseorReferralNumber} may be a duplicate of {duplicateCaseIds}. " +
                                                                   "Please review the Referral list to determine if these are true duplicates. Document your findings in the Task Note.";
                }
                else if(possibleDuplicateReferral.Entities.Any())
                {
                    string duplicateReferralIds = string.Join(",", possibleDuplicateReferral.Entities.Select(x => x.ToEntity<ipg_referral>().ipg_referralnumber).ToList<string>());
                    context.OutputParameters["CaseNote"] = $"Possible duplicate Referral. Referral {caseorReferralNumber} may be a duplicate of Referral {duplicateReferralIds}. " +
                    "Please review the Referral list to determine if these are true duplicates. Document your findings in the Task Note.";
                    context.OutputParameters["TaskDescripton"] = $"Referral {caseorReferralNumber} may be a duplicate of {duplicateCaseIds}. " +
                                               "Please review the Referral list to determine if these are true duplicates. Document your findings in the Task Note.";

                }
                else
                {
                    context.OutputParameters["CaseNote"] = $"Possible duplicate Referral. Referral {caseorReferralNumber} may be a duplicate of Case {duplicateCaseIds}. " +
                    "Please review the Referral list to determine if these are true duplicates. Document your findings in the Task Note.";
                    context.OutputParameters["TaskDescripton"] = $"Referral {caseorReferralNumber} may be a duplicate of {duplicateCaseIds}. " +
                           "Please review the Referral list to determine if these are true duplicates. Document your findings in the Task Note.";

                }

                context.OutputParameters["Succeeded"] = false;
            }
            else
            {
                context.OutputParameters["Succeeded"] = true;
            }
        }

        private static EntityCollection GetPossibleDuplicateCase(LocalPluginContext localPluginContext, string targetEntityFirstName, string targetEntityLastName, DateTime targetEntityDOB)
        {
            var queryExpression = new QueryExpression(Incident.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(
                    Incident.Fields.ipg_PatientFirstName,
                    Incident.Fields.ipg_PatientLastName,
                    Incident.Fields.ipg_PatientDateofBirth,
                    Incident.Fields.Title),

                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Incident.ipg_PatientDateofBirth).ToLower(), ConditionOperator.Equal, targetEntityDOB)
                        }
                }
            };

            EntityCollection caseWithSameDOB = localPluginContext.OrganizationService.RetrieveMultiple(queryExpression);

            EntityCollection possibleDuplicateCaseID = new EntityCollection();

            foreach (var caseItem in caseWithSameDOB.Entities)
            {
                var item = caseItem.ToEntity<Incident>();

                string sourceFirstName = item.ipg_PatientFirstName.ToLower();
                string destinationFirstName = targetEntityFirstName.ToLower();

                string sourceLastName = item.ipg_PatientLastName.ToLower();
                string destinationLastName = targetEntityLastName.ToLower();

                if (sourceFirstName.ComputeLevenshteinDistance(destinationFirstName) < 3 && sourceLastName.ComputeLevenshteinDistance(destinationLastName) < 3)
                {
                    possibleDuplicateCaseID.Entities.Add(item);
                }
            }

            return possibleDuplicateCaseID;
        }

        private static EntityCollection GetPossibleDuplicateReferral(LocalPluginContext localPluginContext, string targetEntityFirstName, string targetEntityLastName, DateTime targetEntityDOB, EntityReference referral)
        {
            var queryExpression = new QueryExpression(ipg_referral.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(
                    ipg_referral.Fields.ipg_PatientFirstName,
                    ipg_referral.Fields.ipg_PatientLastName,
                    ipg_referral.Fields.ipg_PatientDateofBirth,
                    ipg_referral.Fields.ipg_referralnumber,
                    ipg_referral.Fields.ipg_referralcasenumber),

                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_referral.ipg_PatientDateofBirth).ToLower(), ConditionOperator.Equal, targetEntityDOB),
                            new ConditionExpression(ipg_referral.Fields.Id, ConditionOperator.NotEqual, referral.Id)
                        }
                }
            };

            EntityCollection caseWithSameDOB = localPluginContext.OrganizationService.RetrieveMultiple(queryExpression);

            EntityCollection possibleDuplicateReferral = new EntityCollection();

            foreach (var referralItem in caseWithSameDOB.Entities)
            {
                var item = referralItem.ToEntity<ipg_referral>();

                string sourceFirstName = item.ipg_PatientFirstName.ToLower();
                string destinationFirstName = targetEntityFirstName.ToLower();

                string sourceLastName = item.ipg_PatientLastName.ToLower();
                string destinationLastName = targetEntityLastName.ToLower();

                if (sourceFirstName.ComputeLevenshteinDistance(destinationFirstName) < 3 && sourceLastName.ComputeLevenshteinDistance(destinationLastName) < 3)
                {
                    possibleDuplicateReferral.Entities.Add(item);
                }
            }

            return possibleDuplicateReferral;
        }

    }
}

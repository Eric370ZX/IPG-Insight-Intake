using Microsoft.Xrm.Sdk;
using System.Linq;

namespace Insight.Intake.Repositories
{
    public class DocumentTypeRepository
    {
        private IOrganizationService _crmService;

        public DocumentTypeRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public ipg_documenttype GetByAbbreviation(string abbreviation)
        {
            using (CrmServiceContext context = new CrmServiceContext(_crmService))
            {
                return context.ipg_documenttypeSet
                    .FirstOrDefault(x => 
                        x.ipg_DocumentTypeAbbreviation == abbreviation &&
                        x.StateCode == ipg_documenttypeState.Active);
            }
        }
    }
}

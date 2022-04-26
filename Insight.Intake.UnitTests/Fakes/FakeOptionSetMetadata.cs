using System.Linq;
using Insight.Intake.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Insight.Intake.UnitTests.Fakes
{
    static class FakeOptionSetMetadata
    {
        public static PicklistAttributeMetadata Gender()
        {
            const string male = "Male", female = "Female";

            return new PicklistAttributeMetadata
            {
                OptionSet = new OptionSetMetadata
                {
                    Name = "gendercode",
                    DisplayName = new Label("Gender", (int)LanguageCodes.US),
                    IsGlobal = true,
                    OptionSetType = OptionSetType.Picklist,
                    Options =
                    {
                        new OptionMetadata(new Label(male, (int)LanguageCodes.US)
                        {
                            UserLocalizedLabel = new LocalizedLabel(male, (int)LanguageCodes.US)
                        }, (int)Genders.Male),
                        new OptionMetadata(new Label(female, (int)LanguageCodes.US)
                        {
                            UserLocalizedLabel = new LocalizedLabel(female, (int)LanguageCodes.US)
                        }, (int)Genders.Female)
                    }
                }
            };
        }

        public static PicklistAttributeMetadata FakeHomeplanNetworkOptionset()
        {
            const string HOME_PLAN_NETWORK = "BCBS";

            return new PicklistAttributeMetadata
            {
                OptionSet = new OptionSetMetadata
                {
                    Name = "ipg_homeplannetwork",
                    DisplayName = new Label("Home Plan Network", (int)LanguageCodes.US),
                    IsGlobal = true,
                    OptionSetType = OptionSetType.Picklist,
                    Options =
                    {
                        new OptionMetadata(new Label(HOME_PLAN_NETWORK, (int)LanguageCodes.US)
                        {
                            UserLocalizedLabel = new LocalizedLabel(HOME_PLAN_NETWORK,(int)LanguageCodes.US)
                        },1)
                    }
                }
            };
        }

        public static int[] OptionSetValues(PicklistAttributeMetadata picklistAttributeMetadata)
        {
            return picklistAttributeMetadata.OptionSet.Options.Select(o => o.Value ?? 0).ToArray();
        }
    }
}

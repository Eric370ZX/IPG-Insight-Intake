using Insight.Intake.Helpers;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Helpers
{
    public class PreImagePostImageHelperTest
    {
        [Fact]
        public void ValidateEntityPreAndPostImageChangesHelper()
        {
            //Arrange
            Incident preImage = new Incident().Fake();
            Incident postImage = new Incident().Fake().WithPatientReference(new Contact().Fake());


            //Act 
            var changes = PreImagePostImageHelper.EntityPreAndPostImageChangesHelper(preImage, postImage);

            //Assert
            Assert.Equal(postImage.Attributes.Count(), changes.Count);
            Assert.True(changes.ContainsKey("incidentid"));
            Assert.True(changes.ContainsKey("ipg_patientid"));
        }
    }
}

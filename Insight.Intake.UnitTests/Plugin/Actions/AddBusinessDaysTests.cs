using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Insight.Intake.Plugin.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Insight.Intake.UnitTests.Plugin.Actions
{
    public class AddBusinessDaysTests
    {
        [Fact]
        public void AddBusinessDays_Add_One_No_Weekend()
        {
                                   
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", null } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();

            DateTime expected = new DateTime(2014, 7, 4, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_Three_With_Weekend()
        {
                                   
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 3 },
                { "ClosuresCalendar", null } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();

            DateTime expected = new DateTime(2014, 7, 8, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_Minus_One_No_Weekend()
        {
                                   
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", -1 },
                { "ClosuresCalendar", null } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();

            DateTime expected = new DateTime(2014, 7, 2, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_Minus_Five_With_Weekend()
        {
                                   
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", -5 },
                { "ClosuresCalendar", null } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();

            DateTime expected = new DateTime(2014, 6, 26, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_One_Non_Full_Day_With_Holiday()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July");
            calendarRule.Attributes.Add("starttime", new DateTime(2014, 7, 4, 11, 0, 0));
            calendarRule.Attributes.Add("duration", 30);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Main Holiday Schedule");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            DateTime expected = new DateTime(2014, 7, 4, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_One_Non_Full_Day_With_Holiday_Plus_Weekend()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July");
            calendarRule.Attributes.Add("starttime", new DateTime(2014, 7, 4, 11, 0, 0));
            calendarRule.Attributes.Add("duration", 30);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Main Holiday Schedule");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 4 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            DateTime expected = new DateTime(2014, 7, 9, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_One_Holiday_Plus_Weekend()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July");
            calendarRule.Attributes.Add("starttime", new DateTime(2014, 7, 4, 0, 0, 0));
            calendarRule.Attributes.Add("duration", 1440);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Main Holiday Schedule");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            DateTime expected = new DateTime(2014, 7, 7, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_One_Non_Full_Day_With_Business_Closure()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July");
            calendarRule.Attributes.Add("starttime", new DateTime(2014, 7, 4, 11, 0, 0));
            calendarRule.Attributes.Add("duration", 240);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Main Holiday Schedule");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            DateTime expected = new DateTime(2014, 7, 4, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_One_With_Business_Closure_Plus_Weekend()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July");
            calendarRule.Attributes.Add("starttime", new DateTime(2014, 7, 4, 0, 0, 0));
            calendarRule.Attributes.Add("duration", 1440);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Main Holiday Schedule");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            DateTime expected = new DateTime(2014, 7, 7, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_Business_Closure_3_Days_Plus_Weekend()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July extended 2nd-4th");
            calendarRule.Attributes.Add("starttime", new DateTime(2014, 7, 2, 0, 0, 0));
            calendarRule.Attributes.Add("duration", 4320);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Business Closure Calendar");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2014, 7, 1, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            DateTime expected = new DateTime(2014, 7, 7, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_Business_Closure_2_Days_No_Weekend()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July extended 2nd-4th");
            calendarRule.Attributes.Add("starttime", new DateTime(2018, 7, 3, 0, 0, 0));
            calendarRule.Attributes.Add("duration", 2880);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Business Closure Calendar");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2018, 7, 2, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            DateTime expected = new DateTime(2018, 7, 5, 8, 48, 0, 0);   
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
        [Fact]
        public void AddBusinessDays_Add_Business_Closure_1_Full_Day_1_Partial_No_Weekend()
        {
            EntityCollection calendarRules = new EntityCollection();
            Entity calendarRule = new Entity
            {
                LogicalName = "calendarRule",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection()
            };
            calendarRule.Attributes.Add("name", "4th of July extended 4th to 5th at 1200");
            calendarRule.Attributes.Add("starttime", new DateTime(2018, 7, 4, 0, 0, 0));
            calendarRule.Attributes.Add("duration", 2160);
            calendarRules.Entities.Add(calendarRule);

            Entity holidayCalendar = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = "calendar",
                Attributes = new AttributeCollection()
            };
            holidayCalendar.Attributes.Add("name", "Business Closure Calendar");
            holidayCalendar.Attributes.Add("calendarrules", calendarRules);
                        
            var inputParameters = new ParameterCollection { 
                { "StartDate", new DateTime(2018, 7, 3, 8, 48, 0, 0)},
                { "BusinessDaysToAdd", 1 },
                { "ClosuresCalendar", holidayCalendar.ToEntityReference() } };

            var outputParameters = new ParameterCollection {};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAddBusinessDays",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            xrmFakedContext.Initialize(new List<Entity> { calendarRule, holidayCalendar });

            // NOTE: While this is during the closure, the current logic only evaluates for full days - there for the 5th is a 'working day' 
            DateTime expected = new DateTime(2018, 7, 5, 8, 48, 0, 0);
            
            var result = xrmFakedContext.ExecutePluginWith<AddBusinessDays>(pluginContext);
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("ResultDate") && pluginContext.OutputParameters["ResultDate"] is DateTime);
            Assert.True((DateTime)pluginContext.OutputParameters["ResultDate"] == expected);
        }
    }
}

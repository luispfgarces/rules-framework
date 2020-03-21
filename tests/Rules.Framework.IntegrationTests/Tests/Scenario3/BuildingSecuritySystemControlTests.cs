namespace Rules.Framework.IntegrationTests.Tests.Scenario3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Core;

    [TestClass]
    public class BuildingSecuritySystemControlTests
    {
        [TestMethod]
        public async Task BuildingSecuritySystem_FireScenario_ReturnsActionsToTrigger()
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.FireSystem;

            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<SecuritySystemConditions>[] expectedConditions = new Condition<SecuritySystemConditions>[]
            {
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.TemperatureCelsius,
                    Value = 100.0m
                },
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.SmokeRate,
                    Value = 55.0m
                },
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.PowerStatus,
                    Value = "Online"
                }
            };

            IRulesDataSource<SecuritySystemActionables, SecuritySystemConditions> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<SecuritySystemActionables, SecuritySystemConditions>($@"{Environment.CurrentDirectory}/Tests/Scenario3/BuildingSecuritySystemControlTests.datasource.json");

            RulesEngine<SecuritySystemActionables, SecuritySystemConditions> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act
            IEnumerable<Rule<SecuritySystemActionables, SecuritySystemConditions>> actual = await rulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            IEnumerable<SecuritySystemAction> securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "CallFireBrigade")
                .And.Contain(ssa => ssa.ActionName == "CallPolice")
                .And.Contain(ssa => ssa.ActionName == "ActivateSprinklers")
                .And.HaveCount(3);
        }

        [TestMethod]
        public async Task BuildingSecuritySystem_PowerShutdownScenario_ReturnsActionsToTrigger()
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<SecuritySystemConditions>[] expectedConditions = new Condition<SecuritySystemConditions>[]
            {
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.TemperatureCelsius,
                    Value = 100.0m
                },
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.SmokeRate,
                    Value = 55.0m
                },
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.PowerStatus,
                    Value = "Shutdown"
                }
            };

            IRulesDataSource<SecuritySystemActionables, SecuritySystemConditions> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<SecuritySystemActionables, SecuritySystemConditions>($@"{Environment.CurrentDirectory}/Tests/Scenario3/BuildingSecuritySystemControlTests.datasource.json");

            RulesEngine<SecuritySystemActionables, SecuritySystemConditions> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act
            IEnumerable<Rule<SecuritySystemActionables, SecuritySystemConditions>> actual = await rulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            IEnumerable<SecuritySystemAction> securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.HaveCount(1);
        }

        [TestMethod]
        public async Task BuildingSecuritySystem_PowerFailureScenario_ReturnsActionsToTrigger()
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<SecuritySystemConditions>[] expectedConditions = new Condition<SecuritySystemConditions>[]
            {
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.TemperatureCelsius,
                    Value = 100.0m
                },
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.SmokeRate,
                    Value = 55.0m
                },
                new Condition<SecuritySystemConditions>
                {
                    Type = SecuritySystemConditions.PowerStatus,
                    Value = "Offline"
                }
            };

            IRulesDataSource<SecuritySystemActionables, SecuritySystemConditions> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<SecuritySystemActionables, SecuritySystemConditions>($@"{Environment.CurrentDirectory}/Tests/Scenario3/BuildingSecuritySystemControlTests.datasource.json");

            RulesEngine<SecuritySystemActionables, SecuritySystemConditions> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act
            IEnumerable<Rule<SecuritySystemActionables, SecuritySystemConditions>> actual = await rulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            IEnumerable<SecuritySystemAction> securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.Contain(ssa => ssa.ActionName == "EnableEmergencyPower")
                .And.Contain(ssa => ssa.ActionName == "CallPowerGridPicket");
        }
    }
}
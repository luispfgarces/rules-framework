namespace Rules.Framework.Providers.InMemory.IntegrationTests.Tests.Scenario3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario3;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    public class BuildingSecuritySystemControlTests : BaseScenarioTests
    {
        private readonly InMemoryRulesStorage<SecuritySystemActionables, SecuritySystemConditions> inMemoryRulesStorage;

        public BuildingSecuritySystemControlTests()
        {
            this.inMemoryRulesStorage = new InMemoryRulesStorage<SecuritySystemActionables, SecuritySystemConditions>();

            this.LoadInMemoryStorage<SecuritySystemActionables, SecuritySystemConditions, SecuritySystemAction>(
                DataSourceFilePath,
                this.inMemoryRulesStorage,
                (c) => JsonConvert.DeserializeObject<SecuritySystemAction>((string)c));
        }

        private static string DataSourceFilePath => $@"{Environment.CurrentDirectory}/Scenarios/Scenario3/rules-framework-tests.security-system-actionables.json";

        [Fact]
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

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            RulesEngine<SecuritySystemActionables, SecuritySystemConditions> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetInMemoryDataSource(serviceProvider)
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

        [Fact]
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

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            RulesEngine<SecuritySystemActionables, SecuritySystemConditions> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetInMemoryDataSource(serviceProvider)
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

        [Fact]
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

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            RulesEngine<SecuritySystemActionables, SecuritySystemConditions> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetInMemoryDataSource(serviceProvider)
                .Build();

            // Act
            IEnumerable<Rule<SecuritySystemActionables, SecuritySystemConditions>> actual = await rulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            IEnumerable<SecuritySystemAction> securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.HaveCount(1);
        }
    }
}
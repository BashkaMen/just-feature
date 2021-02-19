using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace JustFeature.CSharp
{
    public class FeatureToggleTests
    {
        public class TestFeature : FeatureToggle.IFeatureKey {}

        
        [Fact]
        public void Enable_and_disable_works()
        {
            FeatureToggle.RegisterAllFeatures(new[] {Assembly.GetExecutingAssembly()}, Feature.Enabled);

            FeatureToggle
                .GetOr<TestFeature>(Feature.Disabled)
                .Should()
                .BeEquivalentTo(Feature.Enabled);

            FeatureToggle.ConfigureFeature<TestFeature>(Feature.Disabled); // configure feature on current context (AsyncLocal)

            FeatureToggle
                .GetOr<TestFeature>(Feature.Enabled)
                .Should()
                .BeEquivalentTo(Feature.Disabled);
        }

        [Fact]
        public async Task Context_are_thread_safe()
        {
            FeatureToggle.RegisterAllFeatures(new[] {Assembly.GetExecutingAssembly()}, Feature.Enabled);
            var random = new Random();

            async Task DoWork()
            {
                await Task.Delay(100);
                var isEnabled = random.NextDouble() > 0.5;
                FeatureToggle.ConfigureFeature<TestFeature>(isEnabled ? Feature.Enabled : Feature.Disabled);
                await Task.Delay(random.Next(100));
                
                var feature = FeatureToggle.GetOr<TestFeature>(isEnabled ? Feature.Disabled : Feature.Enabled);
                feature.Should().BeEquivalentTo(isEnabled ? Feature.Enabled : Feature.Disabled);
            }

            
            var tasks = Enumerable.Range(0, 500).Select(s => DoWork()).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}
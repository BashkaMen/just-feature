using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace JustFeature.CSharp
{

    public class FeatureTests
    {
        private readonly ITestOutputHelper _output;

        public FeatureTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Simple_Usage()
        {
            var feature = Feature.Disabled; // always disabled
            feature = Feature.NewRated(0.3); // enabled with random percent
            feature = Feature.NewCustom(() => DateTime.Now.DayOfWeek == DayOfWeek.Friday); // enabled on friday 13
            //feature = Feature.NewFCustom() // Custom for F#;
            feature = Feature.Enabled; // always enabled

            var result = feature.Run(
                () => 100,
                () => 0
            );

            result.Should().Be(100, "feature is enabled");
        }


        [Theory]
        [InlineData("ru", false)]
        [InlineData("com", true)]
        public void Simple_Usage_2(string brand, bool enabled)
        {
            Feature GetDiscountFeature(string request)
            {
                // some logic of process request for enabling feature
                // if (Request.Header ...)
                // if (DateTime.Now  == ...) 
            
                //return Feature.Enabled;
                //return Feature.Disabled;
                //return Feature.NewRated(0.3);

                return request.Contains("com") ? Feature.Enabled : Feature.Disabled;
            }
            
            var request = $"site.{brand}/api/get-discount";

            var discountFeature = GetDiscountFeature(request);
            
            var discount = discountFeature.Run(
                () => 10,
                () => 20
            );
            
            _output.WriteLine($"You discount are {discount}");

            discountFeature.IsEnabled().Should().Be(enabled);
        }
    }
}
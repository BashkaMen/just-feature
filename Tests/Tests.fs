module Tests

open System
open JustFeature
open Xunit
open Swensen.Unquote


type TestFeature() = interface FeatureToggle.IFeatureKey

[<Fact>]
let ``feature usage`` () =
    let oldCalc() = "old value"
    let newCalc() = "new value"
    
    let calc (feature: Feature) = Feature.run newCalc oldCalc feature
    
    let newCalcFeature(brand) =
        match brand with
        | "com" -> Feature.Enabled
        | _ -> Feature.Disabled


    let feature = newCalcFeature("com")
    
    test <@ feature.IsEnabled() = true @>
    test <@ calc feature = "new value" @>
    
    
    let feature = newCalcFeature("ru")
    
    test <@ feature.IsEnabled() = false @>
    test <@ calc feature = "old value" @>
        
    

[<Fact>]
let ``feature toggle usage`` () =
    FeatureToggle.configureFeature<TestFeature>(Feature.Enabled)
    
    let oldCalc() = "old value"
    let newCalc() = "new value"
    
    let feature = FeatureToggle.getOr<TestFeature>(Feature.Disabled)
    let calc() = FeatureToggle.getOr<TestFeature>(Feature.Disabled).Run(newCalc, oldCalc)
    
    test <@ feature.IsEnabled() = true @>
    test <@ calc() = "new value" @>
    
    FeatureToggle.configureFeature<TestFeature>(Feature.Disabled)
    let feature = FeatureToggle.getOr<TestFeature>(Feature.Disabled)
    
    test <@ calc() = "old value" @>
    test <@ feature.IsEnabled() = false @>
    
    
    
    
    
    
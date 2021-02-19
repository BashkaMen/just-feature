namespace JustFeature

open System
open System.Runtime.CompilerServices
open FeatureModule


[<Extension>]
type CSharpApiExt =
    
    [<Extension>]
    static member IsEnabled(feature: Feature) = isEnabled feature
    
    
    [<Extension>]
    static member Run(feature: Feature, ifEnable: Func<'a>, ifDisable: Func<'a>) =
        run feature (ifEnable.Invoke) (ifDisable.Invoke)
        
        
    [<Extension>]
    static member Run(feature: Feature, ifEnable: Action, ifDisable: Action) =
        run feature ifEnable.Invoke ifDisable.Invoke
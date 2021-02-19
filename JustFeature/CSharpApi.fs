namespace JustFeature

open System
open System.Runtime.CompilerServices
open Feature


[<Extension>]
type CSharpApiExt =
    
    [<Extension>]
    static member IsEnabled(feature: Feature) = isEnabled feature
    
    
    [<Extension>]
    static member Run(feature: Feature, ifEnable: Func<'a>, ifDisable: Func<'a>) =
        run (ifEnable.Invoke) (ifDisable.Invoke) feature
        
        
    [<Extension>]
    static member Run(feature: Feature, ifEnable: Action, ifDisable: Action) =
        run ifEnable.Invoke ifDisable.Invoke feature